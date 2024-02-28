using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientDto
    {
        [JsonProperty(nameof(FullName))]
        [MinLength(5)]
        [MaxLength(100)]
        [Required]
        public string FullName { get; set; }

        [Required]
        [JsonProperty(nameof(AgeGroup))]
        [Range(0, 2)]
        public int AgeGroup { get; set; }

        [Required]
        [JsonProperty(nameof(Gender))]
        [Range(0, 1)]
        public int Gender { get; set; }

        [JsonProperty(nameof(Medicines))]
        public int[] Medicines { get; set; }
    }
}

//•	FullName – text with length [5, 100] (required)
//•	AgeGroup – AgeGroup enum (Child = 0, Adult, Senior)(required)
//•	Gender – Gender enum (Male = 0, Female)(required)
//•	PatientsMedicines - collection of type PatientMedicine