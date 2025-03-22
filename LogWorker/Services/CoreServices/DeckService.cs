using Domain.Interfaces;

namespace LogWorker.Services.CoreServices
{
    public class DeckService : IDeckService
    {
        public DeckService()
        {

        }

        public string FetchDeck(string line, StreamReader sr, string delimeter)
        {
            if (line.Contains("<== Event_SetDeckV2")) {
                var json = sr.ReadLine();

                return json + delimeter;
            }

            return string.Empty;
        }
    }
}
