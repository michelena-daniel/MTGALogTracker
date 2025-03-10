using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Settings;
using LogWorker.Helpers;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LogWorker.Services
{
    public class LogReaderService : ILogReaderService
    {
        private readonly LogPathOptions _options;
        private readonly ILogger<LogReaderService> _logger;
        private readonly IMapper _mapper;

        private readonly IUserInfoRepository _userInfoRepository;
        private readonly IPlayerRankRepository _playerRankRepository;

        public LogReaderService(IOptions<LogPathOptions> options, ILogger<LogReaderService> logger, IPlayerRankRepository playerRankRepository, IUserInfoRepository userInfoRepository, IMapper mapper)
        {
            _options = options.Value;
            _logger = logger;
            _playerRankRepository = playerRankRepository;
            _userInfoRepository = userInfoRepository;
            _mapper = mapper;
        }

        public async Task ProcessLogFile()
        {
            _logger.LogInformation("Start MTGA log reader");
            var logTransaction = new LogTransaction();
            var logPath = _options.MtgaLogPath;
            if (!File.Exists(logPath))
            {
                _logger.LogWarning("Log path not found.");
                return;
            }
            // Read MTGA user's log
            using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                string previousLine = string.Empty;
                string currentUser = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var userLog = FetchUserInfo(line, previousLine, sr);
                    logTransaction.UserInfo += userLog;                   
                    if (!string.IsNullOrEmpty(userLog))
                    {
                        var userDto = JsonSerializer.Deserialize<UserInfoDto>(userLog);
                        currentUser = userDto.UserNameWithCode;
                    }
                    logTransaction.RankLogs += FetchRankInfo(line, previousLine, sr, currentUser);
                    previousLine = line;
                }
            }
            // Deserialize into objects
            var rankDetailsList = JsonHelper.DeserializeJsonObjects<PlayerRankDto>(logTransaction.RankLogs, @"\{""constructedSeasonOrdinal"".*?\}");
            var userInfo = JsonHelper.DeserializeJsonObjects<UserInfoDto>(logTransaction.UserInfo, @"\{""UserNameWithCode"".*?\}");
            // Write into db
            await WriteUserInfo(userInfo);
            await WriteRankDetails(rankDetailsList);
            _logger.LogInformation("Reader complete for users: {Usernames}", string.Join(",", userInfo.Select(u => u.UserNameWithCode)));
            _logger.LogInformation("Reader complete for ranks with the following logIds: {logIds}", string.Join(",", rankDetailsList.Select(r => r.LogId)));
        }

        private async Task WriteRankDetails(List<PlayerRankDto> rankDetails)
        {
            //avoid rank duplicates
            rankDetails = rankDetails
                .Where(r => !string.IsNullOrWhiteSpace(r.CurrentUser))
                .GroupBy(u => u.LogId)
                .Select(g => g.First())
                .ToList();
            // add userId's to rank details
            var userNames = rankDetails.Select(r => r.CurrentUser).Distinct().ToList();
            var users = await _userInfoRepository.GetUserIdsByUserNames(userNames);
            var rankDetailsEntity = _mapper.Map<List<PlayerRank>>(rankDetails);
            foreach (var rank in rankDetailsEntity)
            {
                var user = users.FirstOrDefault(u => u.UserNameWithCode == rank.CurrentUser);
                if (user == null)
                {
                    _logger.LogWarning($"Skipping rank: No matching user found for '{rank.CurrentUser}'.");
                    continue;
                }
                rank.UserId = user.UserId;
            }
            // filter out ranks whhich already exist on database
            var logIds = rankDetailsEntity.Select(x => x.LogId).ToList();
            var existingPlayerRanks = await _playerRankRepository.GetRanksByLogIds(logIds);
            var existingPlayerRanksLogIds = existingPlayerRanks.Select(epr => epr.LogId);
            var filteredRankDetails = rankDetailsEntity.Where(rd => !existingPlayerRanksLogIds.Contains(rd.LogId)).ToList();

            if(filteredRankDetails == null || !filteredRankDetails.Any())
            {
                _logger.LogInformation("No new player ranks to insert");
                return;
            }

            await _playerRankRepository.AddRanksAsync(filteredRankDetails);
            _logger.LogInformation("Added new player ranks with the following log id's: {logIds}", string.Join(",", filteredRankDetails.Select(r => r.LogId)));
        }

        private async Task WriteUserInfo(List<UserInfoDto> userInfo)
        {
            // avoid user duplicates
            userInfo = userInfo
                .GroupBy(u => u.UserName)
                .Select(g => g.First())
                .ToList();
            // filter users who already exist on database (being a local log file we shouldn't have a massive load so I fetch everything)
            var existingUsers = await _userInfoRepository.GetAllUsers();
            var existingUserNames = existingUsers.Select(u => u.UserNameWithCode).ToHashSet(); // Hash set for speed + uniqueness
            var userInfoEntities = _mapper.Map<List<UserInfo>>(userInfo).Where(x => !existingUserNames.Contains(x.UserNameWithCode)).ToList();

            if(userInfoEntities == null || !userInfoEntities.Any())
            {
                _logger.LogInformation("No new users to write");
                return;
            }

            await _userInfoRepository.AddUsersAsync(userInfoEntities);
            _logger.LogInformation("Added users: {users}", string.Join(",", userInfoEntities.Select(u => u.UserNameWithCode)));
        }

        private string FetchUserInfo(string line, string previousLine, StreamReader sr)
        {
            if(line.Contains("[Accounts - Login] Logged in successfully. Display Name:"))
            {
                var userNameWithCode = line.Replace("[Accounts - Login] Logged in successfully. Display Name:", "");
                var userNameSplit = userNameWithCode.Split("#");
                var userName = userNameSplit[0];
                var userCode = userNameSplit[1];
                var userInfo = new UserInfoDto { UserNameWithCode = userNameWithCode, UserName = userName, UserCode = userCode };
                var userInfoJson = JsonSerializer.Serialize(userInfo);

                return userInfoJson;
            }

            return string.Empty;
        }

        private string FetchRankInfo(string line, string previousLine, StreamReader sr, string currentUser)
        {
            var timestampMatch = Regex.Match(previousLine, @"\[UnityCrossThreadLogger\](\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2})");
            if (line.Contains("Rank_GetCombinedRankInfo") && timestampMatch.Success)
            {
                var timeStampString = previousLine.Replace("[UnityCrossThreadLogger]", "");
                var timeStamp = DateTime.ParseExact(timeStampString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var logId = line.Replace("<== Rank_GetCombinedRankInfo", "").Replace("(", "").Replace(")", "").Trim();
                var nextLine = sr.ReadLine();
                var result = nextLine == null ? string.Empty : nextLine.Remove(nextLine.Length-1, 1)+$",\"timeStamp\":\"{timeStamp}\",\"logId\":\"{logId}\",\"user\":\"{currentUser}\"}}";
                return result;
            }

            return string.Empty;
        }        
    }
}
