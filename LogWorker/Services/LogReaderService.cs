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
        private readonly IMatchRepository _matchRepository;

        private const string _delimeter = "\n---JSON-END---\n";
        private string _currentMtgArenaId = "";
        private string _currentPlayerName = "";

        public LogReaderService(IOptions<LogPathOptions> options, ILogger<LogReaderService> logger, IPlayerRankRepository playerRankRepository, IUserInfoRepository userInfoRepository, IMatchRepository matchRepository, IMapper mapper)
        {
            _options = options.Value;
            _logger = logger;
            _playerRankRepository = playerRankRepository;
            _userInfoRepository = userInfoRepository;
            _matchRepository = matchRepository;
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
                while ((line = sr.ReadLine()) != null)
                {
                    var loginLog = FetchUserInfoOnLogin(line, previousLine, sr);
                    var authenticationLog = FetchUserInfoOnAuthenticate(line, previousLine, sr);
                    logTransaction.UserInfo += loginLog;
                    logTransaction.Authentications += authenticationLog;
                    logTransaction.RankLogs += FetchRankInfo(line, previousLine, sr);
                    logTransaction.Matches += FetchMatches(line, previousLine, sr);
                    previousLine = line;
                }
            }
            // Deserialize into objects
            var rankDetailsList = JsonHelper.DeserializeJsonObjects<PlayerRankDto>(logTransaction.RankLogs, _delimeter);
            var userInfo = JsonHelper.DeserializeJsonObjects<UserInfoDto>(logTransaction.UserInfo, _delimeter);
            var matches = JsonHelper.DeserializeJsonObjects<MatchDto>(logTransaction.Matches, _delimeter);
            var authentications = JsonHelper.DeserializeJsonObjects<AuthenticateLogDto>(logTransaction.Authentications, _delimeter);
            // map
            var matchesMapped = MapMatches(matches);            
            // Write into db           
            if(authentications.Count > 0)
            {
                await AddAuthenticatedUsersIfNotExists(authentications);
                await UpdateUserNameWithCodeIfExists(userInfo);
                await WriteMatches(matchesMapped);
                await WriteRankDetails(rankDetailsList);
            }                   

            _logger.LogInformation("Reader complete");
        }

        private async Task WriteRankDetails(List<PlayerRankDto> rankDetails)
        {
            if (rankDetails == null || !rankDetails.Any())
            {
                _logger.LogInformation("No ranks to write.");
                return;
            }
            //avoid rank duplicates
            rankDetails = rankDetails
                .Where(r => !string.IsNullOrWhiteSpace(r.LogId))
                .GroupBy(u => u.LogId)
                .Select(g => g.First())
                .ToList();
            // get users login times
            var authenticatedUsers = await _userInfoRepository.GetAllUsers();
            var loginEntries = authenticatedUsers
                .Where(u => u.LastLogin != null)
                .OrderBy(u => u.LastLogin)
                .ToList();
            var storedIds = loginEntries.Select(l => l.MtgaInternalUserId);
            var logIds = rankDetails.Select(r => r.LogId).ToList();
            var existingRanks = await _playerRankRepository.GetRanksByLogIds(logIds);
            var existingLogIds = existingRanks.Select(r => r.LogId).ToList();
            var filteredRankDetails = rankDetails.Where(r => !existingLogIds.Contains(r.LogId)).ToList();
            if (filteredRankDetails == null || filteredRankDetails.Count == 0)
                return;
            var ranksToRemove = new List<PlayerRankDto>();
            foreach (var rank in filteredRankDetails)
            {
                var timestamp = rank.TimeStamp;
                
                if(rank.MtgArenaUserId == "REFETCH")
                {
                    if (!string.IsNullOrEmpty(rank.PlayerName))
                    {
                        var user = await _userInfoRepository.GetUserByUserNameWithoutCode(rank.PlayerName);
                        if (user != null)
                        {
                            rank.MtgArenaUserId = user.MtgaInternalUserId;
                        }
                    }
                    else
                    {
                        rank.MtgArenaUserId = string.Empty;
                    }                                        
                }

                if (string.IsNullOrEmpty(rank.MtgArenaUserId) && !string.IsNullOrEmpty(rank.PlayerName))
                {
                    var closestUser = loginEntries
                        .Where(u => u.LastLogin != null && u.UserName != null)
                        .Where(u => u.LastLogin <= timestamp && u.UserName == rank.PlayerName)
                        .OrderByDescending(u => u.LastLogin)
                        .FirstOrDefault();                    
                    rank.MtgArenaUserId = closestUser?.MtgaInternalUserId ?? "";
                    var lastRankedUser = await _playerRankRepository.GetLastRankedUserByMtgId(rank.MtgArenaUserId);
                    if (!string.IsNullOrEmpty(rank.MtgArenaUserId) && lastRankedUser != null)
                    {   
                        var lastRankedUsertotalConstructedMatches = lastRankedUser.ConstructedMatchesWon + lastRankedUser.ConstructedMatchesLost + lastRankedUser.ConstructedMatchesDrawn;
                        var rankConstructedMatches = rank.ConstructedMatchesWon + rank.ConstructedMatchesLost + rank.ConstructedMatchesDrawn;
                        var lastRankedUsertotalLimitedMatches = lastRankedUser.LimitedMatchesWon + lastRankedUser.LimitedMatchesLost;
                        var rankLimitedMatches = rank.LimitedMatchesWon + rank.LimitedMatchesLost;
                        if(lastRankedUsertotalConstructedMatches == rankConstructedMatches && lastRankedUsertotalLimitedMatches == rankLimitedMatches)
                        {
                            ranksToRemove.Add(rank);
                        }
                    }
                }

                var userExists = await _userInfoRepository.GetUserByMtgArenaId(rank.MtgArenaUserId);
                if (userExists == null)
                {
                    _logger.LogWarning($"Skipping rank {rank.LogId}: MtgArenaUserId '{rank.MtgArenaUserId}' does not exist in Users table.");
                    ranksToRemove.Add(rank);
                }
            }
            filteredRankDetails = filteredRankDetails.Except(ranksToRemove).ToList();
            if (filteredRankDetails == null || filteredRankDetails.Count == 0)
            {
                _logger.LogInformation("No ranks to write.");
                return;
            }

            var rankDetailsEntities = _mapper.Map<List<PlayerRank>>(filteredRankDetails);
            await _playerRankRepository.AddRanksAsync(rankDetailsEntities);
            _logger.LogInformation("Added new player ranks with the following log id's: {logIds}", string.Join(",", filteredRankDetails.Select(r => r.LogId)));
        }

        private async Task WriteMatches(List<Domain.Entities.Match> matches)
        {
            if (matches == null || !matches.Any())
            {
                _logger.LogInformation("No matches to write.");
                return;
            }
            //Determine home user
            var mtgArenaIds = matches.SelectMany(m => new[] { m.PlayerOneMtgaId, m.PlayerTwoMtgaId })
                                .Where(id => !string.IsNullOrWhiteSpace(id))
                                .Distinct()
                                .ToList();
            var matchingUsers = await _userInfoRepository.GetUsersByMtgArenaIds(mtgArenaIds);
            var usersDict = matchingUsers.ToDictionary(u => u.MtgaInternalUserId, u => u);
            foreach (var match in matches)
            {
                var matchUsers = new List<UserInfo>();
                if (usersDict.TryGetValue(match.PlayerOneMtgaId, out var userOne))
                    matchUsers.Add(userOne);
                if (usersDict.TryGetValue(match.PlayerTwoMtgaId, out var userTwo))
                    matchUsers.Add(userTwo);

                match.HomeUser = matchUsers.Count switch
                {
                    0 => "",
                    1 => matchUsers[0].MtgaInternalUserId,
                    2 => matchUsers.MaxBy(mu => mu.LastLogin)?.MtgaInternalUserId ?? "",
                    _ => ""
                };

                if (match.HomeUser == "")
                    _logger.LogWarning("Users of match id {MatchId} not found in database", match.MatchId);
            }

            // Filter out existing matches
            var matchIds = matches.Select(x => x.MatchId).ToList();
            var existingMatches = await _matchRepository.GetMatchesByMatchIds(matchIds);
            var existingMatchesIds = existingMatches.Select(m => m.MatchId).ToList();
            var filteredMatches = matches.Where(m => !existingMatchesIds.Contains(m.MatchId)).ToList();

            if(filteredMatches == null || filteredMatches.Count == 0)
            {
                return;
            }

            await _matchRepository.AddMatchesAsync(filteredMatches);
            _logger.LogInformation("Added new matches");
        }

        private async Task AddAuthenticatedUsersIfNotExists(List<AuthenticateLogDto> authentications)
        {
            if (authentications == null || authentications.Count == 0)
            {
                _logger.LogInformation("No new authenticated users read to add.");
                return;
            }
            // avoid duplicates
            var uniqueAuthentications = authentications
                .Where(a => !string.IsNullOrWhiteSpace(a.AuthenticateResponse.ClientId))
                .OrderByDescending(a => long.Parse(a.Timestamp))
                .GroupBy(a => a.AuthenticateResponse.ClientId)
                .Select(g => g.First())
                .Select(a => new UserInfo
                {
                    UserName = a.AuthenticateResponse.ScreenName,
                    MtgaInternalUserId = a.AuthenticateResponse.ClientId,
                    LastLogin = new DateTime(long.Parse(a.Timestamp), DateTimeKind.Utc)
                })
                .ToList();

            if (uniqueAuthentications.Count == 0)
            {
                _logger.LogInformation("No valid authenticated users found.");
                return;
            }
            // Fetch existing users from DB
            var mtgaIds = uniqueAuthentications.Select(a => a.MtgaInternalUserId).ToList();
            var existingUsers = await _userInfoRepository.GetUsersByMtgArenaIds(mtgaIds);
            var existingUserIds = new HashSet<string>(existingUsers.Select(u => u.MtgaInternalUserId).Where(id => !string.IsNullOrWhiteSpace(id)));

            // Filter out users that already exist in DB
            var newUsers = uniqueAuthentications
                .Where(a => !existingUserIds.Contains(a.MtgaInternalUserId))
                .ToList();

            if (newUsers.Any())
            {
                await _userInfoRepository.AddUsersAsync(newUsers);
                _logger.LogInformation("Added {Count} new authenticated users.", newUsers.Count);
            }
            else
            {
                _logger.LogInformation("No new authenticated users to add.");
                var loginUpdatesApplied = await _userInfoRepository.UpdateLoginTimes(uniqueAuthentications);
                _logger.LogInformation("Updated login times applied: {count} .", loginUpdatesApplied);
            }
        }

        private async Task UpdateUserNameWithCodeIfExists(List<UserInfoDto> userInfoDtos)
        {
            var usersToUpdate = new List<UserInfo>();
            if (userInfoDtos != null && userInfoDtos.Count > 0)
            {
                var usernamesWithoutCode = userInfoDtos.Select(u => u.UserName).ToList();
                var users = await _userInfoRepository.GetUsersByUserNameWithoutCode(usernamesWithoutCode);                
                foreach(var user in users)
                {
                    // TODO: since this applies by Name (not unique) it could wrongly assign values if two users with the same username exist on the system, need to fix that
                    if (user.UserNameWithCode == null && user.MtgaInternalUserId != null)
                    {
                        user.UserNameWithCode = userInfoDtos.Where(u => u.UserName == user.UserName).Select(u => u.UserNameWithCode).FirstOrDefault();
                        user.UserCode = userInfoDtos.Where(u => u.UserName == user.UserName).Select(u => u.UserCode).FirstOrDefault();
                        if (!string.IsNullOrEmpty(user.UserNameWithCode))
                            usersToUpdate.Add(user);                        
                    }
                }
            }

            if(usersToUpdate.Count > 0)
            {
                await _userInfoRepository.UpdateUsernamesWithoutCode(usersToUpdate);
            }
        }

        private List<Domain.Entities.Match> MapMatches(List<MatchDto> matches)
        {
            var matchesEntities = new List<Domain.Entities.Match>();
            foreach(var match in matches)
            {
                var matchEntity = new Domain.Entities.Match();
                matchEntity.RequestId = match.RequestId;
                matchEntity.TransactionId = match.TransactionId;
                matchEntity.TimeStamp = match.Timestamp; // need to convert this to DateTime first
                matchEntity.MatchId = match.MatchGameRoomStateChangedEvent.GameRoomInfo.FinalMatchResult.MatchId;
                matchEntity.MatchCompletedReason = match.MatchGameRoomStateChangedEvent.GameRoomInfo.FinalMatchResult.MatchCompletedReason;
                matchEntity.PlayerOneMtgaId = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers[0].UserId;
                matchEntity.PlayerOneName = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers[0].PlayerName;
                matchEntity.PlayerTwoMtgaId = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers[1].UserId;
                matchEntity.PlayerTwoName = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers[1].PlayerName;
                var gameRoomConfigMatchId = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.MatchId; // should be the same as the finalMatchResult one
                var reservedPlayers = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers;
                var result = match.MatchGameRoomStateChangedEvent.GameRoomInfo.FinalMatchResult.ResultList[0];
                if (result.Result != "ResultType_WinLoss")
                {
                    matchEntity.IsDraw = true;
                }
                else
                {
                    matchEntity.WinningTeamId = result.WinningTeamId;
                }

                matchesEntities.Add(matchEntity);
            }

            return matchesEntities;
        }

        private string FetchUserInfoOnLogin(string line, string previousLine, StreamReader sr)
        {
            if(line.Contains("[Accounts - Login] Logged in successfully. Display Name:"))
            {
                var userNameWithCode = line.Replace("[Accounts - Login] Logged in successfully. Display Name:", "").Trim();
                var userNameSplit = userNameWithCode.Split("#");
                var userName = userNameSplit[0];
                var userCode = userNameSplit[1];
                var userInfo = new UserInfoDto { UserNameWithCode = userNameWithCode, UserName = userName, UserCode = userCode };
                var userInfoJson = JsonSerializer.Serialize(userInfo);

                _currentPlayerName = userName;
                _currentMtgArenaId = "REFETCH";

                return userInfoJson+_delimeter;
            }

            return string.Empty;
        }

        private string FetchUserInfoOnAuthenticate(string line, string previousLine, StreamReader sr)
        {
            if (line.Contains("AuthenticateResponse"))
            {
                var nextLine = sr.ReadLine();
                try
                {
                    var authenticationInfo = JsonSerializer.Deserialize<AuthenticateLogDto>(nextLine);
                    _currentMtgArenaId = authenticationInfo != null ? authenticationInfo.AuthenticateResponse.ClientId : "";
                    _currentPlayerName = authenticationInfo != null ? authenticationInfo.AuthenticateResponse.ScreenName : "";
                }
                catch(Exception ex)
                {
                    _logger.LogError("Error deserializing authentication info on fetch: {ex} .", ex);
                }                
                return nextLine + _delimeter;
            }

            return string.Empty;
        }

        private string FetchRankInfo(string line, string previousLine, StreamReader sr)
        {
            var timestampMatch = Regex.Match(previousLine, @"\[UnityCrossThreadLogger\]?\s*(\d{2}/\d{2}/\d{4} \d{1,2}:\d{2}:\d{2})");
            if (line.Contains("Rank_GetCombinedRankInfo") && timestampMatch.Success)
            {
                var timeStampString = previousLine.Replace("[UnityCrossThreadLogger]", "");
                var timeStamp = DateTime.ParseExact(timeStampString, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                var logId = line.Replace("<== Rank_GetCombinedRankInfo", "").Replace("(", "").Replace(")", "").Trim();
                var nextLine = sr.ReadLine();
                var result = nextLine == null ? string.Empty : nextLine.Remove(nextLine.Length-1, 1)+$",\"timeStamp\":\"{timeStamp}\",\"logId\":\"{logId}\",\"playerName\":\"{_currentPlayerName}\",\"mtgArenaUserId\":\"{_currentMtgArenaId}\"}}";
                return result+_delimeter;
            }

            return string.Empty;
        }

        private string FetchMatches(string line, string previousLine, StreamReader sr)
        {
            if(line.Contains("\"stateType\": \"MatchGameRoomStateType_MatchCompleted\", \"finalMatchResult\":"))
            {
                return line+_delimeter;
            }
            return string.Empty;
        }
    }
}
