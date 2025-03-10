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
            var userInfo = JsonHelper.DeserializeJsonObjects<UserInfoDto>(logTransaction.UserInfo, @"\{""UserName"".*?\}");
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
                var nextLine = sr.ReadLine();
                var result = nextLine == null ? string.Empty : nextLine.Remove(nextLine.Length-1, 1)+$",\"timeStamp\":\"{timeStamp}\",\"user\":\"{currentUser}\"}}";
                return result;
            }

            return string.Empty;
        }        
    }
}
