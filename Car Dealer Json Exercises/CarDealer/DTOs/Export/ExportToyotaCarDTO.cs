using Newtonsoft.Json;

namespace CarDealer.DTOs.Export
{
    public class ExportToyotaCarDTO
    {
        [JsonProperty(nameof(Id))]
        public int Id { get; set; }

        [JsonProperty(nameof(Make))]
        public string Make { get; set; } = null!;

        [JsonProperty(nameof(Model))]
        public string Model { get; set; } = null!;

        [JsonProperty(nameof(TraveledDistance))]
        public long TraveledDistance { get; set; }
    }
}
