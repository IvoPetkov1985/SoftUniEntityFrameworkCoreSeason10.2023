using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Export
{
    public class ExportPartDTO
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [JsonProperty("Price")]
        public string Price { get; set; } = null!;
    }

    public class ExportCarDTO
    {
        [JsonProperty("Make")]
        public string Make { get; set; } = null!;

        [JsonProperty("Model")]
        public string Model { get; set; } = null!;

        [JsonProperty("TraveledDistance")]
        public long TraveledDistance { get; set; }
    }

    public class CarPartsDTO
    {
        [JsonProperty("car")]
        public ExportCarDTO Car { get; set; } = null!;

        [JsonProperty("parts")]
        public ExportPartDTO[] Parts { get; set; } = null!;
    }
}
