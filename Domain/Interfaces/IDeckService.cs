namespace Domain.Interfaces
{
    public interface IDeckService
    {
        string FetchDeck(string line, StreamReader sr, string delimeter);
    }
}
