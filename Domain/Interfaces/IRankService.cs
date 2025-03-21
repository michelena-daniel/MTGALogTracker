using Domain.Models;

namespace Domain.Interfaces
{
    public interface IRankService
    {
        string FetchRankInfo(string line, string previousLine, StreamReader sr, LogAuthenticationState logState, string delimeter);
        Task WriteRankDetails(List<PlayerRankDto> rankDetails);
    }
}
