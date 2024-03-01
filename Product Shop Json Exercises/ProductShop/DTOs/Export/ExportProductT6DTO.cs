using Newtonsoft.Json;

namespace ProductShop.DTOs.Export
{
    public class ExportProductT6DTO
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("buyerFirstName")]
        public string? BuyerFName { get; set; }

        [JsonProperty("buyerLastName")]
        public string BuyerLName { get; set; } = null!;
    }

    public class ExportUserT6DTO
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; } = null!;

        [JsonProperty("soldProducts")]
        public ExportProductT6DTO[] SoldProducts { get; set; } = null!;
    }
}
