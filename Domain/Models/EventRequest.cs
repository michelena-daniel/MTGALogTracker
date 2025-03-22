using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class EventRequest
    {
        public string? MtgArenaId { get; set; }
        public string? UserName { get; set; }
        public string? EventName { get; set; }
        public string? EntryCurrencyType { get; set; }
        public int EntryCurrencyPaid { get; set; }
        public string? CustomTokenId { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
