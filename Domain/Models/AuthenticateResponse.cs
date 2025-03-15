using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class AuthenticateResponse
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }

        [JsonPropertyName("screenName")]
        public string ScreenName { get; set; }
    }
}