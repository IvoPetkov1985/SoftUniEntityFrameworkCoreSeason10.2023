using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string LogoUrl { get; set; } = null!;

        [Required]
        [StringLength(3)]
        public string Initials { get; set; } = null!;

        [Required]
        public decimal Budget { get; set; }

        [Required]
        public int PrimaryKitColorId { get; set; }

        [ForeignKey(nameof(PrimaryKitColorId))]
        public virtual Color PrimaryKitColor { get; set; } = null!;

        [Required]
        public int SecondaryKitColorId { get; set; }

        [ForeignKey(nameof(SecondaryKitColorId))]
        public virtual Color SecondaryKitColor { get; set; } = null!;

        [Required]
        public int TownId { get; set; }

        [ForeignKey(nameof(TownId))]
        public virtual Town Town { get; set; } = null!;

        [InverseProperty(nameof(Game.HomeTeam))]
        public virtual ICollection<Game> HomeGames { get; set; } = new List<Game>();

        [InverseProperty(nameof(Game.AwayTeam))]
        public virtual ICollection<Game> AwayGames { get; set; } = new List<Game>();

        public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
