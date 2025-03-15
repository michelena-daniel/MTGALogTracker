using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserInfoRepository
    {
        Task AddUsersAsync(List<UserInfo> users);
        Task<UserInfo> GetUserByUserNameWithCode(string userNameWithCode);
        Task<List<UserInfo>> GetAllUsers();
        Task<List<UserInfo>> GetUsersByUserNameWithoutCode(string userNameWithoutCode);
        Task<UserInfo> GetUserByMtgArenaId(string mtgArenaId);
        Task<List<UserInfo>> GetUsersByMtgArenaIds(List<string> mtgArenaIds);
        Task<List<UserInfo>> GetUsersByUserNameWithCode(List<string> userNamesWithCode);
        Task<List<UserInfo>> GetUsersByUserNameWithoutCode(List<string> userNamesWithoutCode);
        Task UpdateUsernamesWithoutCode(List<UserInfo> users);
        Task<UserInfo?> GetUserByUserNameWithoutCode(string userName);
        Task<int> UpdateLoginTimes(List<UserInfo> users);
    }
}
