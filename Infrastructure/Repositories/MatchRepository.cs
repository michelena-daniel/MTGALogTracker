using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddMatchesAsync(List<Match> matches)
        {
            if(matches == null || matches.Count == 0)
            {
                return;
            }

            await _context.AddRangeAsync(matches);
        }

        public async Task<List<Match>> GetMatchesByMatchIds(List<string> matchIds)
        {
            return await _context.Matches.Where(m => matchIds.Contains(m.MatchId)).ToListAsync();
        }

        public async Task<List<Match>> GetMatchesByMtgArenaId(string mtgArenaId)
        {
            return await _context.Matches.Where(m => m.HomeUser == mtgArenaId).ToListAsync();
        }

        public async Task<List<Match>> GetUnknownMatches()
        {
            return await _context.Matches.Where(m => m.HomeUser == "").ToListAsync();
        }

        public async Task UpdateMatchesWithHomeUser(List<Match> matchesToUpdate)
        {
            foreach (var match in matchesToUpdate)
            {
                await _context.Matches
                    .Where(m => m.MatchId == match.MatchId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(m => m.HomeUser, match.HomeUser)
                    );
            }
        }

        public async Task<List<Match>> GetAllMatches()
        {
            return await _context.Matches.ToListAsync();
        }

        public async Task<Match?> GetLastMatchPlayed()
        {
            return await _context.Matches
                .OrderByDescending(m => m.TimeStamp)
                .FirstOrDefaultAsync();
        }
    }
}
