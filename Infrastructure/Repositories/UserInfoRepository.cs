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

        public async Task<UserInfo> GetUserByUserNameWithCode(string userNameWithCode)
        {
            return await _context.Users
                .Where(u => u.UserNameWithCode == userNameWithCode).FirstOrDefaultAsync();
        }

        public async Task<List<UserInfo>> GetUsersByUserNameWithoutCode(string userNameWithoutCode)
        {
            return await _context.Users.Where(u => u.UserName == userNameWithoutCode).ToListAsync();
        }

        public async Task<UserInfo> GetUserByMtgArenaId(string mtgArenaId)
        {
            return await _context.Users.Where(u => u.MtgaInternalUserId == mtgArenaId).FirstOrDefaultAsync();
        }

        public async Task<List<UserInfo>> GetUsersByMtgArenaIds(List<string> mtgArenaIds)
        {
            if (!mtgArenaIds.Any()) return new List<UserInfo>();

            return await _context.Users
                .Where(u => u.MtgaInternalUserId != null && mtgArenaIds.Contains(u.MtgaInternalUserId))
                .ToListAsync();
        }

        public async Task<List<UserInfo>> GetUsersByUserNameWithCode(List<string> userNamesWithCode)
        {
            if (!userNamesWithCode.Any()) return new List<UserInfo>();

            return await _context.Users
                .Where(u => u.UserNameWithCode != null && userNamesWithCode.Contains(u.UserNameWithCode))
                .ToListAsync();
        }

        public async Task<List<UserInfo>> GetUsersByUserNameWithoutCode(List<string> userNamesWithoutCode)
        {
            if (!userNamesWithoutCode.Any()) return new List<UserInfo>();

            return await _context.Users
                .Where(u => u.UserName != null && userNamesWithoutCode.Contains(u.UserName))
                .ToListAsync();
        }

        public async Task<UserInfo?> GetUserByUserNameWithoutCode(string userName)
        {
            return await _context.Users
                .Where(u => u.UserName == userName).FirstOrDefaultAsync();
        }

        public async Task UpdateUsernamesWithoutCode(List<UserInfo> users)
        {
            if (users == null || users.Count == 0)
                return;

            var updatesApplied = 0;
            foreach (var user in users)
            {
                if (string.IsNullOrWhiteSpace(user.MtgaInternalUserId) || string.IsNullOrWhiteSpace(user.UserNameWithCode))
                    continue;

                updatesApplied += await _context.Users
                    .Where(dbUser => dbUser.MtgaInternalUserId == user.MtgaInternalUserId && string.IsNullOrWhiteSpace(dbUser.UserNameWithCode))
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(dbUser => dbUser.UserNameWithCode, user.UserNameWithCode)
                        .SetProperty(dbUser => dbUser.UserCode, user.UserCode)
                    );
            }
        }

        public async Task<int> UpdateLoginTimes(List<UserInfo> users)
        {
            if (users == null || users.Count == 0)
                return 0;

            var userIds = users.Select(u => u.MtgaInternalUserId).ToList();
            var userUpdates = users.ToDictionary(u => u.MtgaInternalUserId, u => u.LastLogin);
            var usersToUpdate = await _context.Users
                .Where(u => userIds.Contains(u.MtgaInternalUserId))
                .ToListAsync();
            foreach (var user in usersToUpdate)
            {
                if (userUpdates.TryGetValue(user.MtgaInternalUserId, out var lastLogin))
                {
                    user.LastLogin = lastLogin;
                }
            }
            return await _context.SaveChangesAsync();
        }
    }
}
