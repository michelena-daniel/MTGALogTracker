using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPlayerRankRepository
    {
        Task AddRanksAsync(List<PlayerRank> ranks);
        Task<List<PlayerRank>> GetRanksByLogIds(List<string> logIds);
        Task<List<PlayerRank>> GetPlayerRanksByPlayerName(string playerNameWithCode);
    }
}
