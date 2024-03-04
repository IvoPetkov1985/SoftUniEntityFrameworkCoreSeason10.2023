using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Balance { get; set; }

        public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}

// UserId, Username, Password, Email, Name, Balance
