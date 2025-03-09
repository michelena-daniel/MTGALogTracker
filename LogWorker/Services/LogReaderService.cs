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
            var playerLogs = new List<string>();
            var logPath = _options.MtgaLogPath;
            var timeStamp = DateTime.Now;

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
                    var timestampMatch = Regex.Match(previousLine, @"\[UnityCrossThreadLogger\](\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2})");
                    if (line.Contains("Rank_GetCombinedRankInfo") && timestampMatch.Success)
                    {
                        var timeStampString = previousLine.Replace("[UnityCrossThreadLogger]", "");
                        timeStamp = DateTime.ParseExact(timeStampString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        var nextLine = sr.ReadLine();
                        playerLogs.Add(nextLine);
                    }

                    previousLine = line;
                }
            }

            string extractedLogs = string.Join(" ", playerLogs);
            Console.WriteLine("Extracted logs:", extractedLogs);

            // Deserialize into objects
            var jsonPattern = new Regex(@"\{""constructedSeasonOrdinal"".*?\}", RegexOptions.Singleline);
            var matches = jsonPattern.Matches(extractedLogs);
            var rankDetailsList = new List<PlayerRankDto>();

            foreach (Match match in matches)
            {
                try
                {
                    // Deserialize JSON into RankDetails object
                    PlayerRankDto rankDetails = JsonSerializer.Deserialize<PlayerRankDto>(match.Value);
                    if (rankDetails != null)
                    {
                        rankDetailsList.Add(rankDetails);
                        Console.WriteLine($"Rango: {rankDetails.ConstructedClass}, Nivel: {rankDetails.ConstructedLevel.ToString()}, Step: {rankDetails.ConstructedStep}, Partidas ganadas: {rankDetails.ConstructedMatchesWon}, Partidas perdidas: {rankDetails.ConstructedMatchesLost}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }
        }
    }
}
