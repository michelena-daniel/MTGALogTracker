using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPlayerRankRepository
    {
        Task AddRanksAsync(List<PlayerRank> ranks);
    }
}
