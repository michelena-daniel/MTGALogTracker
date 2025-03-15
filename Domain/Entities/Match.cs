namespace Domain.Entities
{
    public class Match
    {
        public string MatchId { get; set; }
        public int RequestId { get; set; }
        public string TransactionId { get; set; }
        public string TimeStamp { get; set; }
        public string MatchCompletedReason { get; set; }
        public bool IsDraw { get; set; }
        public int WinningTeamId {get; set;}
        public string PlayerOneName { get; set; }
        public string PlayerTwoName { get; set; }
        public string PlayerOneMtgaId { get; set; }
        public string PlayerTwoMtgaId { get; set; }
        public string? HomeUser { get; set; }
        public UserInfo User { get; set; }
    }
}
