using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Deck;
using Domain.Models.Settings;
using LogWorker.Helpers;
using Microsoft.Extensions.Options;

namespace LogWorker.Services
{
    public class LogReaderService : ILogReaderService
    {
        private readonly LogPathOptions _options;
        private readonly ILogger<LogReaderService> _logger;

        private readonly IUserInfoService _userInfoService;
        private readonly IRankService _rankService;
        private readonly IMatchService _matchService;
        private readonly IEventService _eventService;
        private readonly IDeckService _deckService;

        private const string _delimeter = "\n---JSON-END---\n";

        public LogReaderService(IOptions<LogPathOptions> options, ILogger<LogReaderService> logger, IRankService rankService, IUserInfoService userInfoService, IMatchService matchService, IDeckService deckService, IEventService eventService)
        {
            _options = options.Value;
            _logger = logger;
            _rankService = rankService;
            _userInfoService = userInfoService;
            _matchService = matchService;
            _rankService = rankService;
            _deckService = deckService;
            _eventService = eventService;
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
            // 1 - Read
            var logState = new LogAuthenticationState();
            var eventState = new EventState();
            using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                string previousLine = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    var loginLog = _userInfoService.FetchUserInfoOnLogin(line, _delimeter, logState);
                    var authenticationLog = _userInfoService.FetchUserInfoOnAuthenticate(line, sr, _delimeter, logState);

                    logTransaction.UserInfo += loginLog;
                    logTransaction.Authentications += authenticationLog;

                    logTransaction.EventInfo += _eventService.FetchEventJoin(line, sr, _delimeter, logState, eventState);
                    logTransaction.DeckInfo += _deckService.FetchDeck(line, sr, _delimeter, eventState);

                    logTransaction.RankLogs += _rankService.FetchRankInfo(line, previousLine, sr, logState, _delimeter);
                    logTransaction.Matches += _matchService.FetchMatches(line, _delimeter, eventState);
                    previousLine = line;
                }
            }
            // 2 - Deserialize JSONs
            var rankDetailsList = JsonHelper.DeserializeJsonObjects<PlayerRankDto>(logTransaction.RankLogs, _delimeter);
            var userInfo = JsonHelper.DeserializeJsonObjects<UserInfoDto>(logTransaction.UserInfo, _delimeter);
            var matches = JsonHelper.DeserializeJsonObjects<MatchDto>(logTransaction.Matches, _delimeter);
            var authentications = JsonHelper.DeserializeJsonObjects<AuthenticateLogDto>(logTransaction.Authentications, _delimeter);
            var eventsInfo = JsonHelper.DeserializeJsonObjects<EventRequest>(logTransaction.EventInfo, _delimeter);
            var decksInfo = JsonHelper.DeserializeJsonObjects<EventSetDeckV2Dto>(logTransaction.DeckInfo, _delimeter);
            var matchesMapped = _matchService.MapMatches(matches);
            
            // 3 - Write         
            if (authentications.Count > 0)
            {
                await _userInfoService.AddAuthenticatedUsersIfNotExists(authentications);
                await _userInfoService.UpdateUserNameWithCodeIfExists(userInfo);
                await _matchService.WriteMatches(matchesMapped);
                await _rankService.WriteRankDetails(rankDetailsList);
            }

            _logger.LogInformation("Reader complete");
        }
    }
}
