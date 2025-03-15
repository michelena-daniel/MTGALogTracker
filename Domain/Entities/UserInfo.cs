using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserInfo
    {
        [Key]
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string MtgaInternalUserId { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        public string? UserNameWithCode { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string UserName { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        public string? UserCode { get; set; }        
        public DateTime? LastLogin { get; set; }
        public ICollection<PlayerRank> PlayerRanks { get; set; } = new List<PlayerRank>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
