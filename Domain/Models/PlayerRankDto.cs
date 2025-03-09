using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class PlayerRankDto
    {
        [JsonPropertyName("constructedSeasonOrdinal")]
        public int ConstructedSeasonOrdinal { get; set; }

        [JsonPropertyName("constructedClass")]
        public string ConstructedClass { get; set; }

        [JsonPropertyName("constructedLevel")]
        public int ConstructedLevel { get; set; }

        [JsonPropertyName("constructedStep")]
        public int ConstructedStep { get; set; }

        [JsonPropertyName("constructedMatchesWon")]
        public int ConstructedMatchesWon { get; set; }

        [JsonPropertyName("constructedMatchesLost")]
        public int ConstructedMatchesLost { get; set; }

        [JsonPropertyName("constructedMatchesDrawn")]
        public int ConstructedMatchesDrawn { get; set; }

        [JsonPropertyName("limitedSeasonOrdinal")]
        public int LimitedSeasonOrdinal { get; set; }

        [JsonPropertyName("limitedClass")]
        public string LimitedClass { get; set; }

        [JsonPropertyName("limitedLevel")]
        public int LimitedLevel { get; set; }

        [JsonPropertyName("limitedStep")]
        public int LimitedStep { get; set; }

        [JsonPropertyName("limitedMatchesWon")]
        public int LimitedMatchesWon { get; set; }

        [JsonPropertyName("limitedMatchesLost")]
        public int LimitedMatchesLost { get; set; }

        // **Metadata fields (not part of JSON but added manually)**
        [JsonIgnore] // Prevent these from being written back to JSON
        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public string LogId { get; set; }
    }
}
