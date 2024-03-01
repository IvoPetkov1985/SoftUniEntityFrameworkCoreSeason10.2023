using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Car")]
    public class ImportCarDTO
    {
        [XmlElement("make")]
        public string Make { get; set; } = null!;

        [XmlElement("model")]
        public string Model { get; set; } = null!;

        [XmlElement("traveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        [XmlArrayItem("partId")]
        public ImportPartId[] Parts { get; set; } = null!;
    }

    [XmlType("partId")]
    public class ImportPartId
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
