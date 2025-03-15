using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMatchRepository
    {
        Task AddMatchesAsync(List<Entities.Match> matches);
        Task<List<Entities.Match>> GetMatchesByMatchIds(List<string> matchIds);
        Task<List<Entities.Match>> GetUnknownMatches();
        Task UpdateMatchesWithHomeUser(List<Match> matches);
        Task<List<Entities.Match>> GetAllMatches();
        Task<Match?> GetLastMatchPlayed();
    }
}
