using Newtonsoft.Json;

namespace CarDealer.DTOs.Export
{
    public class ExportLocSupplierDTO
    {
        [JsonProperty(nameof(Id))]
        public int Id { get; set; }

        [JsonProperty(nameof(Name))]
        public string Name { get; set; } = null!;

        [JsonProperty(nameof(PartsCount))]
        public int PartsCount { get; set; }
    }
}
