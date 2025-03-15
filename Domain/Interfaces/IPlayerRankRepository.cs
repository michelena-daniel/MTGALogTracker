using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPlayerRankRepository
    {
        Task AddRanksAsync(List<PlayerRank> ranks);
        Task<List<PlayerRank>> GetRanksByLogIds(List<string> logIds);
        Task<PlayerRank> GetLastRankedUser();
        Task<List<PlayerRank>> GetLastRankedUsersByMtgArenaIds(List<string> mtgArenaIds);
        Task<List<PlayerRank>> GetRanksByMtgArenaId(string mtgArenaId);
        Task<PlayerRank?> GetLastRankedUserByMtgId(string mtgArenaId);
    }
}
