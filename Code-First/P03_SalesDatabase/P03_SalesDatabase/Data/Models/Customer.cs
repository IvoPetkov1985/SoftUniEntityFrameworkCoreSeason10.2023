using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace P03_SalesDatabase.Data.Models
{
    public class Customer
    {
        public Customer()
        {
            Sales = new List<Sale>();
        }

        [Key]
        public int CustomerId { get; set; }

        [Required]
        [Unicode]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [Unicode(false)]
        [MaxLength(80)]
        public string Email { get; set; } = null!;

        [Required]
        public string CreditCardNumber { get; set; } = null!;

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
