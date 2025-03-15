using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class GameRoomConfig
    {
        [JsonPropertyName("matchId")]
        public string MatchId { get; set; }

        [JsonPropertyName("reservedPlayers")]
        public List<MatchPlayerDto> ReservedPlayers { get; set; }
    }
}