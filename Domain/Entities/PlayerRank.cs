using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PlayerRank
    {
        [Key]
        public int RankId { get; set; }
        [Required]
        public int ConstructedSeasonOrdinal { get; set; }
        [Required]
        public string ConstructedClass { get; set; }
        [Required]
        public int ConstructedLevel { get; set; }
        [Required]
        public int ConstructedStep { get; set; }
        [Required]
        public int ConstructedMatchesWon { get; set; }
        [Required]
        public int ConstructedMatchesLost { get; set; }
        [Required]
        public int ConstructedMatchesDrawn { get; set; }
        [Required]
        public int LimitedSeasonOrdinal { get; set; }
        [Required]
        public string LimitedClass { get; set; }
        [Required]
        public int LimitedLevel { get; set; }
        [Required]
        public int LimitedStep { get; set; }
        [Required]
        public int LimitedMatchesWon { get; set; }
        [Required]
        public int LimitedMatchesLost { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        [Required]
        public string LogId { get; set; }
        [ForeignKey("UserInfo")]
        public int UserId { get; set; }
        public UserInfo User { get; set; }
    }
}
