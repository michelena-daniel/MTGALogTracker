using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class MatchGameResult
    {
        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("winningTeamId")]
        public int WinningTeamId { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}