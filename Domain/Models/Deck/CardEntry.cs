using System.Text.Json.Serialization;

namespace Domain.Models.Deck
{
    public class CardEntry
    {
        [JsonPropertyName("cardId")]
        public int CardId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}