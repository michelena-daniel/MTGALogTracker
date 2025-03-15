using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class FinalMatchResult
    {
        [JsonPropertyName("matchId")]
        public string MatchId { get; set; }

        [JsonPropertyName("matchCompletedReason")]
        public string MatchCompletedReason { get; set; }

        [JsonPropertyName("resultList")]
        public List<MatchGameResult> ResultList { get; set; }
    }
}