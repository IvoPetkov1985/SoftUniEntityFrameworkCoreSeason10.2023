using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace P03_SalesDatabase.Data.Models
{
    public class Store
    {
        public Store()
        {
            Sales = new List<Sale>();
        }

        [Key]
        public int StoreId { get; set; }

        [Required]
        [Unicode]
        [MaxLength(80)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
