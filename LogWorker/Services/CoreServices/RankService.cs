using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogWorker.Services.CoreServices
{
    public class RankService : IRankService
    {
        private IPlayerRankRepository _playerRankRepository;
        private IUserInfoRepository _userInfoRepository;
        private ILogger<RankService> _logger;
        private IMapper _mapper;

        public RankService(IPlayerRankRepository playerRankRepository, IUserInfoRepository userInfoRepository, ILogger<RankService> logger, IMapper mapper)
        {
            _playerRankRepository = playerRankRepository;
            _userInfoRepository = userInfoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public string FetchRankInfo(string line, string previousLine, StreamReader sr, LogAuthenticationState logState, string delimeter)
        {
            var timestampMatch = Regex.Match(previousLine, @"\[UnityCrossThreadLogger\]?\s*(\d{2}/\d{2}/\d{4} \d{1,2}:\d{2}:\d{2})");
            if (line.Contains("Rank_GetCombinedRankInfo") && timestampMatch.Success)
            {
                var timeStampString = previousLine.Replace("[UnityCrossThreadLogger]", "");
                var timeStamp = DateTime.ParseExact(timeStampString, "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
                var logId = line.Replace("<== Rank_GetCombinedRankInfo", "").Replace("(", "").Replace(")", "").Trim();
                var nextLine = sr.ReadLine();
                var result = nextLine == null ? string.Empty : nextLine.Remove(nextLine.Length - 1, 1) + $",\"timeStamp\":\"{timeStamp}\",\"logId\":\"{logId}\",\"playerName\":\"{logState.UserName}\",\"mtgArenaUserId\":\"{logState.MtgArenaId}\"}}";
                return result + delimeter;
            }

            return string.Empty;
        }

        public async Task WriteRankDetails(List<PlayerRankDto> rankDetails)
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

                if (rank.MtgArenaUserId == "REFETCH")
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
                        if (lastRankedUsertotalConstructedMatches == rankConstructedMatches && lastRankedUsertotalLimitedMatches == rankLimitedMatches)
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
    }
}
