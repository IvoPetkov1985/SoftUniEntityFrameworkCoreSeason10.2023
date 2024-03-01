using Newtonsoft.Json;

namespace ProductShop.DTOs.Export
{
    public class Task8ProductDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }

    public class Task8NestedSoldProductsInfo
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("products")]
        public Task8ProductDTO[] Products { get; set; }
    }

    public class Task8UserDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; }

        [JsonProperty("soldProducts")]
        public Task8NestedSoldProductsInfo SoldProducts { get; set; }
    }

    public class UsersInfoDTO
    {
        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }

        [JsonProperty("users")]
        public Task8UserDTO[] Users { get; set; }
    }
}
