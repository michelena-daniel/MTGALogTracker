using Domain.Models;
using Domain.Models.Settings;
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
        public LogReaderService(IOptions<LogPathOptions> options, ILogger<LogReaderService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async void ProcessLogFile()
        {
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
                    logTransaction.UserInfo += FetchUserInfo(line, previousLine, sr);
                    logTransaction.RankLogs += FetchRankInfo(line, previousLine, sr);
                    previousLine = line;
                }
            }

            // Deserialize into objects
            var rankDetailsList = DeserializeRankLogs(logTransaction.RankLogs);
            var userInfo = DeserializeUserInfo(logTransaction.UserInfo);
        }

        private string FetchUserInfo(string line, string previousLine, StreamReader sr)
        {
            if(line.Contains("[Accounts - Login] Logged in successfully. Display Name:"))
            {
                var userNameWithCode = line.Replace("[Accounts - Login] Logged in successfully. Display Name:", "");
                var userNameSplit = userNameWithCode.Split("#");
                var userName = userNameSplit[0];
                var userCode = userNameSplit[1];
                var userInfo = new UserInfoDto { UserName = userName, UserCode = userCode };
                var userInfoJson = JsonSerializer.Serialize(userInfo);

                return userInfoJson;
            }

            return string.Empty;
        }

        private string FetchRankInfo(string line, string previousLine, StreamReader sr)
        {
            var timestampMatch = Regex.Match(previousLine, @"\[UnityCrossThreadLogger\](\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2})");
            if (line.Contains("Rank_GetCombinedRankInfo") && timestampMatch.Success)
            {
                var timeStampString = previousLine.Replace("[UnityCrossThreadLogger]", "");
                var timeStamp = DateTime.ParseExact(timeStampString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var nextLine = sr.ReadLine();

                return nextLine == null ? string.Empty : nextLine;
            }

            return string.Empty;
        }

        private List<PlayerRankDto> DeserializeRankLogs(string rankLogs)
        {
            var jsonPattern = new Regex(@"\{""constructedSeasonOrdinal"".*?\}", RegexOptions.Singleline);
            var matches = jsonPattern.Matches(rankLogs);
            var rankDetailsList = new List<PlayerRankDto>();

            foreach (Match match in matches)
            {
                try
                {
                    var rankDetails = JsonSerializer.Deserialize<PlayerRankDto>(match.Value);
                    if (rankDetails != null)
                    {
                        rankDetailsList.Add(rankDetails);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }

            return rankDetailsList;
        }

        private List<UserInfoDto> DeserializeUserInfo(string userLogs)
        {
            var jsonPattern = new Regex(@"\{""UserName"".*?\}", RegexOptions.Singleline);
            var matches = jsonPattern.Matches(userLogs);
            var userInfoList = new List<UserInfoDto>();

            foreach (Match match in matches)
            {
                try
                {
                    var userInfo = JsonSerializer.Deserialize<UserInfoDto>(match.Value);
                    if (userInfo != null)
                    {
                        userInfoList.Add(userInfo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }

            return userInfoList;
        }
    }
}
