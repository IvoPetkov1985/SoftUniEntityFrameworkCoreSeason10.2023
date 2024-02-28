using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmacyDto
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [XmlElement("PhoneNumber")]
        [Required]
        [RegularExpression(@"^\(\d{3}\)\ \d{3}\-\d{4}$")]
        [StringLength(14)]
        public string PhoneNumber { get; set; } = null!;

        [XmlAttribute("non-stop")]
        [Required]
        public string IsNonStop { get; set; }

        [XmlArray("Medicines")]
        [XmlArrayItem("Medicine")]
        public ImportMedicineDto[] Medicines { get; set; }
    }
}

//•	Name – text with length [2, 50] (required)
//•	PhoneNumber – text with length 14. (required)
//o All phone numbers must have the following structure: three digits enclosed in parentheses, followed by a space, three more digits, a hyphen, and four final digits: 
//	Example-> (123) 456 - 7890
//•	IsNonStop – bool  (required)
//•	Medicines - collection of type Medicine
