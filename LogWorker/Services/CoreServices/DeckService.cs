using Domain.Interfaces;
using Domain.Models;
using Domain.Models.Deck;
using System.Text.Json;

namespace LogWorker.Services.CoreServices
{
    public class DeckService : IDeckService
    {
        private ILogger<DeckService> _logger;
        public DeckService(ILogger<DeckService> logger)
        {
            _logger = logger;
        }

        public string FetchDeck(string line, StreamReader sr, string delimeter, EventState eventState)
        {
            if (line.Contains("<== Event_SetDeckV2")) {
                var json = sr.ReadLine();
                try
                {
                    var deckDetails = JsonSerializer.Deserialize<EventSetDeckV2Dto>(json);
                    var deckId = deckDetails.CourseDeckSummary.DeckId;
                    var deckName = deckDetails.CourseDeckSummary.Name;
                    if(!string.IsNullOrEmpty(deckId) && !string.IsNullOrEmpty(deckName))
                    {
                        eventState.UpdateEventStateDeck(deckId, deckName);
                    }      

                }
                catch(Exception ex)
                {
                    _logger.LogError("Failed to deserialize deck details in order to assign current deck.");
                }

                return json + delimeter;
            }

            return string.Empty;
        }
    }
}
