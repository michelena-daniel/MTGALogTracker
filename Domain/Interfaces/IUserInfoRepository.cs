using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserInfoRepository
    {
        Task AddUsersAsync(List<UserInfo> users);
        Task<List<UserInfo>> GetUsersByUserNames(List<string> userNames);
        Task<List<UserInfo>> GetAllUsers();
    }
}
