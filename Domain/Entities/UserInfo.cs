using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public required string UserNameWithCode { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public required string UserName { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public required string UserCode { get; set; }
        public ICollection<PlayerRank> PlayerRanks { get; set; } = new List<PlayerRank>();
    }
}
