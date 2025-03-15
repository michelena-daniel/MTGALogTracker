using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class MatchGameRoomStateChangedEvent
    {
        [JsonPropertyName("gameRoomInfo")]
        public GameRoomInfo GameRoomInfo { get; set; }        
    }
}