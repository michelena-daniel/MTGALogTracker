using Domain.Models;

namespace Domain.Interfaces
{
    public interface IDeckService
    {
        string FetchDeck(string line, StreamReader sr, string delimeter, EventState eventState);
    }
}
