using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public virtual ICollection<Town> Towns { get; set; } = new List<Town>();
    }
}
