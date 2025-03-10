using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserInfo
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public required string UserNameWithCode { get; set; }
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string UserCode { get; set; }
        public ICollection<PlayerRank> PlayerRanks { get; set; } = new List<PlayerRank>();
    }
}
