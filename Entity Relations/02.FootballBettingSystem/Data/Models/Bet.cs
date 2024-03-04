using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Bet
    {
        [Key]
        public int BetId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Prediction { get; set; } = null!;

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [Required]
        public int GameId { get; set; }

        public virtual Game Game { get; set; } = null!;
    }
}

// BetId, Amount, Prediction, DateTime, UserId, GameId
