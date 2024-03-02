using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace P03_SalesDatabase.Data.Models
{
    public class Product
    {
        public Product()
        {
            Sales = new List<Sale>();
        }

        [Key]
        public int ProductId { get; set; }

        [Required]
        [Unicode]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        public double Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
