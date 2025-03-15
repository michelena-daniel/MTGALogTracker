using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class ConnectionInfo
    {
        [JsonPropertyName("connectionState")]
        public string ConnectionState { get; set; }
    }
}