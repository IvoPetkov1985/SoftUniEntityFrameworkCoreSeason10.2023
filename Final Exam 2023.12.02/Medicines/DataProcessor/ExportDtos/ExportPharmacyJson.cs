using Newtonsoft.Json;

namespace Medicines.DataProcessor.ExportDtos
{
    public class ExportPharmacyJson
    {
        [JsonProperty(nameof(Name))]
        public string Name { get; set; } = null!;

        [JsonProperty(nameof(PhoneNumber))]
        public string PhoneNumber { get; set; } = null!;
    }
}
