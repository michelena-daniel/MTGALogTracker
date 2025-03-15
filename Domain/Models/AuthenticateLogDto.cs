using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class AuthenticateLogDto
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        [JsonPropertyName("requestId")]
        public int RequestId { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("authenticateResponse")]
        public AuthenticateResponse AuthenticateResponse { get; set; }
    }
}
