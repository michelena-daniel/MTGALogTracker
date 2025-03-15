namespace Domain.Models
{
    public class UserInfoDto
    {
        public string? UserNameWithCode { get; set; }
        public required string UserName { get; set; }
        public string? UserCode { get; set; }
        public string MtgaInternalUserId { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
