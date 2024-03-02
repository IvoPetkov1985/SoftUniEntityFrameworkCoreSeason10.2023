using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_HospitalDatabase.Data.Models
{
    public class PatientMedicament
    {
        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;

        [Required]
        public int MedicamentId { get; set; }

        [ForeignKey(nameof(MedicamentId))]
        public virtual Medicament Medicament { get; set; } = null!;
    }
}
