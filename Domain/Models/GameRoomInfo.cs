using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class GameRoomInfo
    {
        [JsonPropertyName("gameRoomConfig")]
        public GameRoomConfig GameRoomConfig { get; set; }

        [JsonPropertyName("stateType")]
        public string StateType { get; set; }

        [JsonPropertyName("finalMatchResult")]
        public FinalMatchResult FinalMatchResult { get; set; }
    }
}