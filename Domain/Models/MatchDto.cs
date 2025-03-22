using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MatchDto
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }
        [JsonPropertyName("requestId")]
        public int RequestId { get; set; }
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
        [JsonPropertyName("matchGameRoomStateChangedEvent")]
        public MatchGameRoomStateChangedEvent MatchGameRoomStateChangedEvent { get; set; }
        public string? EventType { get; set; }
        public string? DeckId { get; set; }
        public string? DeckName { get; set; }
    }
}
