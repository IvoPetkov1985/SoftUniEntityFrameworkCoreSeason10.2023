using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    public class ImportCategoryDTO
    {
        [JsonProperty("Name")]
        public string? Name { get; set; }
    }
}
