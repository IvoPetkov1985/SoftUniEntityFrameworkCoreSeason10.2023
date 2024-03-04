using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        [Required]
        public int HomeTeamId { get; set; }

        [ForeignKey(nameof(HomeTeamId))]
        public virtual Team HomeTeam { get; set; } = null!;

        [Required]
        public int AwayTeamId { get; set; }

        [ForeignKey(nameof(AwayTeamId))]
        public virtual Team AwayTeam { get; set; } = null!;

        [Required]
        public int HomeTeamGoals { get; set; }

        [Required]
        public int AwayTeamGoals { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public double HomeTeamBetRate { get; set; }

        [Required]
        public double AwayTeamBetRate { get; set; }

        [Required]
        public double DrawBetRate { get; set; }

        [Required]
        public string Result { get; set; } = null!;

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; } = new List<PlayerStatistic>();

        public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();

        // public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    }
}

// – GameId, HomeTeamId, AwayTeamId, HomeTeamGoals, AwayTeamGoals, DateTime, HomeTeamBetRate, AwayTeamBetRate, DrawBetRate, Result
