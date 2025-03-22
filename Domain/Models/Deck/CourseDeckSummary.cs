using System.Text.Json.Serialization;

namespace Domain.Models.Deck
{
    public class CourseDeckSummary
    {
        public string DeckId { get; set; }
        public string Name { get; set; }
        public List<AttributeDto> Attributes { get; set; }
        public int DeckTileId { get; set; }
    }
}