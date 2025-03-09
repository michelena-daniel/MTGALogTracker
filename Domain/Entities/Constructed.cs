using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Constructed
    {
        public int LogId { get; set; }
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
    }
}
