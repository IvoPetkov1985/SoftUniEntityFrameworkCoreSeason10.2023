using Newtonsoft.Json;

namespace CarDealer.DTOs.Export
{
    public class ExportSaleDTO
    {
        [JsonProperty("car")]
        public ExportCarDTO Car { get; set; } = null!;

        [JsonProperty("customerName")]
        public string CustomerName { get; set; } = null!;

        [JsonProperty("discount")]
        public string Discount { get; set; } = null!;

        [JsonProperty("price")]
        public string Price { get; set; } = null!;

        [JsonProperty("priceWithDiscount")]
        public string PriceWithDiscount { get; set; } = null!;
    }
}
