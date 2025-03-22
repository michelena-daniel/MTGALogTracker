using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class EventJoinDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("request")]
        public string EventRequest {get; set;}
    }
}
