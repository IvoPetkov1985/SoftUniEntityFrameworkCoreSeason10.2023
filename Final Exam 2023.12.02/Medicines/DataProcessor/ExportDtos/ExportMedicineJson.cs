using Newtonsoft.Json;

namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportMedicineJson
    {
        [JsonProperty(nameof(Name))]
        public string Name { get; set; } = null!;

        [JsonProperty(nameof(Price))]
        public string Price { get; set; } = null!;

        [JsonProperty(nameof(Pharmacy))]
        public ExportPharmacyJson Pharmacy { get; set; } = null!;
    }
}
