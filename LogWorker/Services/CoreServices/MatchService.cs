using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Mono.TextTemplating;
using System.Text.Json;

namespace LogWorker.Services.CoreServices
{
    public class MatchService : IMatchService
    {
        private IMatchRepository _matchRepository;
        private IUserInfoRepository _userInfoRepository;
        private ILogger<MatchService> _logger;

        public MatchService(IMatchRepository matchRepository, IUserInfoRepository userInfoRepository, ILogger<MatchService> logger)
        {
            _matchRepository = matchRepository;
            _userInfoRepository = userInfoRepository;
            _logger = logger;
        }

        public string FetchMatches(string line, string delimeter, EventState eventState)
        {
            if (line.Contains("\"stateType\": \"MatchGameRoomStateType_MatchCompleted\", \"finalMatchResult\":"))
            {
                try
                {
                    var match = JsonSerializer.Deserialize<MatchDto>(line);
                    match.DeckName = eventState.DeckName;
                    match.DeckId = eventState.Deck;
                    match.EventType = eventState.EventType;

                    var json = JsonSerializer.Serialize(match);

                    return json;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error serializing match to assign event values.");
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        public async Task WriteMatches(List<Domain.Entities.Match> matches)
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

            if (filteredMatches == null || filteredMatches.Count == 0)
            {
                return;
            }

            await _matchRepository.AddMatchesAsync(filteredMatches);
            _logger.LogInformation("Added new matches");
        }

        public List<Domain.Entities.Match> MapMatches(List<MatchDto> matches)
        {
            var matchesEntities = new List<Domain.Entities.Match>();
            foreach (var match in matches)
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
                matchEntity.DeckId = match.DeckId;
                matchEntity.DeckName = match.DeckName;
                matchEntity.EventType = match.EventType;
                // determine win
                var playerOneTeamId = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers[0].TeamId;
                var winningTeamId = match.MatchGameRoomStateChangedEvent.GameRoomInfo.FinalMatchResult.ResultList[0].WinningTeamId;
                var winnerMtgaArenaId = playerOneTeamId == winningTeamId ? matchEntity.PlayerOneMtgaId : matchEntity.PlayerTwoMtgaId;
                var winnerName = match.MatchGameRoomStateChangedEvent.GameRoomInfo.GameRoomConfig.ReservedPlayers.Where(p => p.UserId == winnerMtgaArenaId).Select(p => p.PlayerName).FirstOrDefault();
                matchEntity.WinnerMtgArenaId = winnerMtgaArenaId;
                matchEntity.WinnerName = winnerName;

                matchesEntities.Add(matchEntity);
            }

            return matchesEntities;
        }
    }
}
