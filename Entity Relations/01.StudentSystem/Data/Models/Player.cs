using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int SquadNumber { get; set; }

        [Required]
        public int Assists { get; set; }

        [Required]
        public int TownId { get; set; }

        [ForeignKey(nameof(TownId))]
        public virtual Town Town { get; set; } = null!;

        [Required]
        public int PositionId { get; set; }

        [ForeignKey(nameof(PositionId))]
        public virtual Position Position { get; set; } = null!;

        [Required]
        public bool IsInjured { get; set; }

        [Required]
        public int TeamId { get; set; }

        public virtual Team Team { get; set; } = null!;

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; } = new List<PlayerStatistic>();

        // public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}

// PlayerId, Name, SquadNumber, Assists, TownId, PositionId, IsInjured, TeamId
