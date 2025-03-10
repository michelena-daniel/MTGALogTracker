using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserInfoRepository
    {
        Task AddUsersAsync(List<UserInfo> users);
        Task<List<UserInfo>> GetUserIdsByUserNames(List<string> userNames);
    }
}
