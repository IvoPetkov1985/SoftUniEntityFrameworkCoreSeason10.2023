using Newtonsoft.Json;

namespace ProductShop.DTOs.Export
{
    public class ExportCategoryDTO
    {
        [JsonProperty("category")]
        public string Category { get; set; } = null!;

        [JsonProperty("productsCount")]
        public int ProductsCount { get; set; }

        [JsonProperty("averagePrice")]
        public string AveragePrice { get; set; } = null!;

        [JsonProperty("totalRevenue")]
        public string TotalRevenue { get; set; } = null!;
    }
}
