using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly ApplicationDbContext _context;

        public UserInfoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUsersAsync(List<UserInfo> users)
        {
            try
            {
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    Console.WriteLine($"Duplicate user detected. Skipping insertion: {pgEx.Message}");
                }
                else
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                }
            }            
        }

        public async Task<List<UserInfo>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<UserInfo>> GetUserIdsByUserNames(List<string> userNames)
        {
            return await _context.Users
                .Where(u => userNames.Contains(u.UserNameWithCode)).ToListAsync();
        }
    }
}
