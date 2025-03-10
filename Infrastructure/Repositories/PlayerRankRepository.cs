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
    }
}
