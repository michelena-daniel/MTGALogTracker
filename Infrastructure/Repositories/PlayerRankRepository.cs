using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PlayerRankRepository : IPlayerRankRepository
    {
        private readonly ApplicationDbContext _context;

        public PlayerRankRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRanksAsync(List<PlayerRank> ranks)
        {
            try
            {
                await _context.PlayerRanks.AddRangeAsync(ranks);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    Console.WriteLine($"Duplicate log id detected. Skipping insertion: {pgEx.Message}");
                }
                else
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                }
            }            
        }
        
        public async Task<List<PlayerRank>> GetRanksByLogIds(List<string> logIds)
        {
            return await _context.PlayerRanks.Where(pr => logIds.Contains(pr.LogId)).ToListAsync();
        }

        public async Task<PlayerRank?> GetLastRankedUser()
        {
            return await _context.PlayerRanks
                .Where(pr => !string.IsNullOrEmpty(pr.MtgArenaUserId))
                .OrderByDescending(pr => pr.TimeStamp)
                .FirstOrDefaultAsync();
        }

        public async Task<PlayerRank?> GetLastRankedUserByMtgId(string mtgArenaId)
        {
            return await _context.PlayerRanks
                .Where(pr => !string.IsNullOrEmpty(pr.MtgArenaUserId) && pr.MtgArenaUserId == mtgArenaId)
                .OrderByDescending(pr => pr.TimeStamp)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PlayerRank>> GetLastRankedUsersByMtgArenaIds(List<string> mtgArenaIds)
        {
            return await _context.PlayerRanks.Where(pr => pr.MtgArenaUserId != null && mtgArenaIds.Contains(pr.MtgArenaUserId)).ToListAsync();
        }

        public async Task<List<PlayerRank>> GetRanksByMtgArenaId(string mtgArenaId)
        {
            return await _context.PlayerRanks.Where(pr => pr.MtgArenaUserId == mtgArenaId).ToListAsync();
        }
    }
}
