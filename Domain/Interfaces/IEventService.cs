using Domain.Models;

namespace Domain.Interfaces
{
    public interface IEventService
    {
        string FetchEventJoin(string line, StreamReader sr, string delimeter, LogAuthenticationState state, EventState eventState);
    }
}
