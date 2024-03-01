using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    public class ImportCategProdDTO
    {
        [JsonProperty(nameof(CategoryId))]
        public int CategoryId { get; set; }

        [JsonProperty(nameof(ProductId))]
        public int ProductId { get; set; }
    }
}
