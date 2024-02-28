using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class ImportMedicineDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [XmlElement("Price")]
        [Required]
        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        [XmlAttribute("category")]
        [Required]
        [Range(0, 4)]
        public int Category { get; set; }

        [XmlElement("ProductionDate")]
        [Required]
        public string ProductionDate { get; set; }

        [XmlElement("ExpiryDate")]
        [Required]
        public string ExpiryDate { get; set; }

        [XmlElement("Producer")]
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Producer { get; set; } = null!;
    }
}

//•	Name – text with length [3, 150] (required)
//•	Price – decimal in range [0.01…1000.00] (required)
//•	Category – Category enum (Analgesic = 0, Antibiotic, Antiseptic, Sedative, Vaccine)(required)
//•	ProductionDate – DateTime (required)
//•	ExpiryDate – DateTime (required)
//•	Producer – text with length [3, 100] (required)