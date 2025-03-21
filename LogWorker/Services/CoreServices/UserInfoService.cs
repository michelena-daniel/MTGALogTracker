using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using System.Text.Json;

namespace LogWorker.Services.CoreServices
{
    public class UserInfoService : IUserInfoService
    {
        private IUserInfoRepository _userInfoRepository;
        private ILogger<UserInfoService> _logger;

        public UserInfoService(IUserInfoRepository userInfoRepository, ILogger<UserInfoService> logger)
        {
            _logger = logger;
            _userInfoRepository = userInfoRepository;
        }

        public string FetchUserInfoOnLogin(string line, string delimeter, LogAuthenticationState logState)
        {
            if (line.Contains("[Accounts - Login] Logged in successfully. Display Name:"))
            {
                var userNameWithCode = line.Replace("[Accounts - Login] Logged in successfully. Display Name:", "").Trim();
                var userNameSplit = userNameWithCode.Split("#");
                var userName = userNameSplit[0];
                var userCode = userNameSplit[1];
                var userInfo = new UserInfoDto { UserNameWithCode = userNameWithCode, UserName = userName, UserCode = userCode };
                var userInfoJson = JsonSerializer.Serialize(userInfo);

                logState.UpdateAuthentication("REFETCH", userName);

                return userInfoJson + delimeter;
            }

            return string.Empty;
        }

        public string FetchUserInfoOnAuthenticate(string line, StreamReader sr, string delimeter, LogAuthenticationState logState)
        {
            if (line.Contains("AuthenticateResponse"))
            {
                var nextLine = sr.ReadLine();
                try
                {
                    var authenticationInfo = JsonSerializer.Deserialize<AuthenticateLogDto>(nextLine);
                    if(authenticationInfo != null)
                    {
                        logState.UpdateAuthentication(authenticationInfo.AuthenticateResponse.ClientId, authenticationInfo.AuthenticateResponse.ScreenName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error deserializing authentication info on fetch: {ex} .", ex);
                }
                return nextLine + delimeter;
            }

            return string.Empty;
        }

        public async Task UpdateUserNameWithCodeIfExists(List<UserInfoDto> userInfoDtos)
        {
            var usersToUpdate = new List<UserInfo>();
            if (userInfoDtos != null && userInfoDtos.Count > 0)
            {
                var usernamesWithoutCode = userInfoDtos.Select(u => u.UserName).ToList();
                var users = await _userInfoRepository.GetUsersByUserNameWithoutCode(usernamesWithoutCode);
                foreach (var user in users)
                {
                    // TODO: since this applies by Name (not unique) it could wrongly assign values if two users with the same username exist on the system, need to fix that
                    if (user.UserNameWithCode == null && user.MtgaInternalUserId != null)
                    {
                        user.UserNameWithCode = userInfoDtos.Where(u => u.UserName == user.UserName).Select(u => u.UserNameWithCode).FirstOrDefault();
                        user.UserCode = userInfoDtos.Where(u => u.UserName == user.UserName).Select(u => u.UserCode).FirstOrDefault();
                        if (!string.IsNullOrEmpty(user.UserNameWithCode))
                            usersToUpdate.Add(user);
                    }
                }
            }

            if (usersToUpdate.Count > 0)
            {
                await _userInfoRepository.UpdateUsernamesWithoutCode(usersToUpdate);
            }
        }

        public async Task AddAuthenticatedUsersIfNotExists(List<AuthenticateLogDto> authentications)
        {
            if (authentications == null || authentications.Count == 0)
            {
                _logger.LogInformation("No new authenticated users read to add.");
                return;
            }
            // avoid duplicates
            var uniqueAuthentications = authentications
                .Where(a => !string.IsNullOrWhiteSpace(a.AuthenticateResponse.ClientId))
                .OrderByDescending(a => long.Parse(a.Timestamp))
                .GroupBy(a => a.AuthenticateResponse.ClientId)
                .Select(g => g.First())
                .Select(a => new UserInfo
                {
                    UserName = a.AuthenticateResponse.ScreenName,
                    MtgaInternalUserId = a.AuthenticateResponse.ClientId,
                    LastLogin = new DateTime(long.Parse(a.Timestamp), DateTimeKind.Utc)
                })
                .ToList();

            if (uniqueAuthentications.Count == 0)
            {
                _logger.LogInformation("No valid authenticated users found.");
                return;
            }
            // Fetch existing users from DB
            var mtgaIds = uniqueAuthentications.Select(a => a.MtgaInternalUserId).ToList();
            var existingUsers = await _userInfoRepository.GetUsersByMtgArenaIds(mtgaIds);
            var existingUserIds = new HashSet<string>(existingUsers.Select(u => u.MtgaInternalUserId).Where(id => !string.IsNullOrWhiteSpace(id)));

            // Filter out users that already exist in DB
            var newUsers = uniqueAuthentications
                .Where(a => !existingUserIds.Contains(a.MtgaInternalUserId))
                .ToList();

            if (newUsers.Any())
            {
                await _userInfoRepository.AddUsersAsync(newUsers);
                _logger.LogInformation("Added {Count} new authenticated users.", newUsers.Count);
            }
            else
            {
                _logger.LogInformation("No new authenticated users to add.");
                var loginUpdatesApplied = await _userInfoRepository.UpdateLoginTimes(uniqueAuthentications);
                _logger.LogInformation("Updated login times applied: {count} .", loginUpdatesApplied);
            }
        }
    }
}
