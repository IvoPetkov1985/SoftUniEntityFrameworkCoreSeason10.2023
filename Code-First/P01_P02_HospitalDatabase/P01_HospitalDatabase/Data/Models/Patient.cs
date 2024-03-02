using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace P01_HospitalDatabase.Data.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [Unicode]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [Unicode]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [Unicode]
        [MaxLength(250)]
        public string Address { get; set; } = null!;

        [Required]
        [Unicode(false)]
        [MaxLength(80)]
        public string Email { get; set; } = null!;

        [Required]
        public bool HasInsurance { get; set; }

        public virtual ICollection<PatientMedicament> Prescriptions { get; set; } = new List<PatientMedicament>();

        public virtual ICollection<Visitation> Visitations { get; set; } = new List<Visitation>();

        public virtual ICollection<Diagnose> Diagnoses { get; set; } = new List<Diagnose>();
    }
}
