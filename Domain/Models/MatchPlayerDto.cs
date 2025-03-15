using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MatchPlayerDto
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }

        [JsonPropertyName("systemSeatId")]
        public int SystemSeatId { get; set; }

        [JsonPropertyName("teamId")]
        public int TeamId { get; set; }

        [JsonPropertyName("connectionInfo")]
        public ConnectionInfo ConnectionInfo { get; set; }

        [JsonPropertyName("courseId")]
        public string CourseId { get; set; }

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }

        [JsonPropertyName("platformId")]
        public string PlatformId { get; set; }

        [JsonPropertyName("eventId")]
        public string EventId { get; set; }
    }
}
