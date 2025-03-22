namespace Domain.Models
{
    public class EventState
    {
        public string EventType { get; private set; } = "";
        public string Deck { get; private set; } = "";
        public string DeckName { get; private set; } = "";

        public void UpdateEventStateType(string eventType)
        {
            EventType = eventType;
        }

        public void UpdateEventStateDeck(string deck, string deckName)
        {
            Deck = deck;
            DeckName = deckName;
        }

        public void Reset()
        {
            EventType = "";
            Deck = "";
        }
    }
}
