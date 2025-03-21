using Domain.Models;

namespace Domain.Interfaces
{
    public interface IUserInfoService
    {
        string FetchUserInfoOnLogin(string line, string delimeter, LogAuthenticationState logState);
        string FetchUserInfoOnAuthenticate(string line, StreamReader sr, string delimeter, LogAuthenticationState logState);
        Task UpdateUserNameWithCodeIfExists(List<UserInfoDto> userInfoDtos);
        Task AddAuthenticatedUsersIfNotExists(List<AuthenticateLogDto> authentications);
    }
}
