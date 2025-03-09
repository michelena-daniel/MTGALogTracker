using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Limited
    {
        public int LogId { get; set; }
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
    }
}
