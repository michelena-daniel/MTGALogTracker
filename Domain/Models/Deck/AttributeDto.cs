using System.Text.Json.Serialization;

namespace Domain.Models.Deck
{
    public class AttributeDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}