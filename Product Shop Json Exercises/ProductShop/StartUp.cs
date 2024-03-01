using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();

            string usersJson = File.ReadAllText("../../../Datasets/users.json");
            string productsJson = File.ReadAllText("../../../Datasets/products.json");
            string categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            string categoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDTO[] userDTOs = JsonConvert.DeserializeObject<ImportUserDTO[]>(inputJson);

            ICollection<User> users = new List<User>();

            foreach (ImportUserDTO userDTO in userDTOs)
            {
                User user = new User();
                user.FirstName = userDTO.FirstName;
                user.LastName = userDTO.LastName;
                user.Age = userDTO.Age;

                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductDTO[] productDTOs = JsonConvert.DeserializeObject<ImportProductDTO[]>(inputJson);

            ICollection<Product> products = new List<Product>();

            foreach (ImportProductDTO productDTO in productDTOs)
            {
                Product product = new Product();
                product.Name = productDTO.Name;
                product.Price = productDTO.Price;
                product.SellerId = productDTO.SellerId;
                product.BuyerId = productDTO.BuyerId;

                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoryDTO[] categoryDTOs = JsonConvert.DeserializeObject<ImportCategoryDTO[]>(inputJson);

            ICollection<Category> categories = new List<Category>();

            foreach (ImportCategoryDTO categoryDTO in categoryDTOs)
            {
                if (string.IsNullOrEmpty(categoryDTO.Name))
                {
                    continue;
                }

                Category category = new Category();
                category.Name = categoryDTO.Name;

                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategProdDTO[] categProdDTOs = JsonConvert.DeserializeObject<ImportCategProdDTO[]>(inputJson);

            ICollection<CategoryProduct> categoriesProducts = new List<CategoryProduct>();

            foreach (ImportCategProdDTO categProdDTO in categProdDTOs)
            {
                CategoryProduct categoryProduct = new CategoryProduct();
                categoryProduct.CategoryId = categProdDTO.CategoryId;
                categoryProduct.ProductId = categProdDTO.ProductId;

                categoriesProducts.Add(categoryProduct);
            }

            context.CategoriesProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            IEnumerable<ExpProductInRangeDTO> products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExpProductInRangeDTO()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .AsEnumerable();

            string result = JsonConvert.SerializeObject(products, Formatting.Indented);
            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUserT6DTO[] users = context.Users
                .AsNoTracking()
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new ExportUserT6DTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                    .Where(p => p.BuyerId != null)
                    .Select(p => new ExportProductT6DTO()
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFName = p.Buyer.FirstName,
                        BuyerLName = p.Buyer.LastName
                    })

                    .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToArray();

            string result = JsonConvert.SerializeObject(users, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include
            });

            return result;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoryDTO[] categories = context.Categories
                .AsNoTracking()
                .Select(c => new ExportCategoryDTO()
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count,
                    AveragePrice = c.CategoriesProducts.Average(p => p.Product.Price).ToString("F2"),
                    TotalRevenue = c.CategoriesProducts.Sum(p => p.Product.Price).ToString("F2")
                })
                .OrderByDescending(c => c.ProductsCount)
                .ToArray();

            string result = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return result;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            Task8UserDTO[] users = context.Users
                .AsNoTracking()
                .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
                .Select(u => new Task8UserDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new Task8NestedSoldProductsInfo()
                    {
                        Count = u.ProductsSold.Where(ps => ps.BuyerId != null).Count(),
                        Products = u.ProductsSold.Where(ps => ps.BuyerId != null)
                        .Select(p => new Task8ProductDTO()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToArray()
                    }

                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToArray();

            UsersInfoDTO usersInfo = new UsersInfoDTO()
            {
                UsersCount = users.Length,
                Users = users
            };

            string result = JsonConvert.SerializeObject(usersInfo, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return result;
        }
    }
}
