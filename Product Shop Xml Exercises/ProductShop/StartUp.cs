using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();

            string usersXml = File.ReadAllText("../../../Datasets/users.xml");
            string productsXml = File.ReadAllText("../../../Datasets/products.xml");
            string categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            string categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Users");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportUserDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportUserDTO[]? userDTOs = (ImportUserDTO[]?)serializer.Deserialize(reader);

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

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Products");

            XmlSerializer deserializer = new XmlSerializer(typeof(ImportProductDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportProductDTO[]? productDTOs = (ImportProductDTO[]?)deserializer.Deserialize(reader);

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

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Categories");

            XmlSerializer deserializer = new XmlSerializer(typeof(ImportCategoryDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportCategoryDTO[]? categoryDTOs = (ImportCategoryDTO[]?)deserializer.Deserialize(reader);

            ICollection<Category> categories = new List<Category>();

            foreach (ImportCategoryDTO categoryDTO in categoryDTOs)
            {
                if (categoryDTO.Name == null)
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

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("CategoryProducts");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategProdDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportCategProdDTO[]? categProdDTOs = (ImportCategProdDTO[]?)serializer.Deserialize(reader);

            ICollection<CategoryProduct> categoriesProducts = new List<CategoryProduct>();

            foreach (ImportCategProdDTO categProdDTO in categProdDTOs)
            {
                if (categProdDTO.CategoryId == null || categProdDTO.ProductId == null)
                {
                    continue;
                }

                CategoryProduct categoryProduct = new CategoryProduct();
                categoryProduct.CategoryId = (int)categProdDTO.CategoryId;
                categoryProduct.ProductId = (int)categProdDTO.ProductId;

                categoriesProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductInRangeDTO[] products = context.Products
                .AsNoTracking()
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductInRangeDTO()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.BuyerId.HasValue ? p.Buyer.FirstName + " " + p.Buyer.LastName : null
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("Products");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportProductInRangeDTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, products, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUserTask6DTO[] users = context.Users
                .AsNoTracking()
                .Where(u => u.ProductsSold.Any())
                .Select(u => new ExportUserTask6DTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                    .Select(p => new ExportSoldProductDTO()
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("Users");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportUserTask6DTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, users, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoryDTO[] categories = context.Categories
                .AsNoTracking()
                .Select(c => new ExportCategoryDTO()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCategoryDTO[]), new XmlRootAttribute("Categories"));

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, categories, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUserT8DTO[] users = context.Users
                .AsNoTracking()
                .Where(u => u.ProductsSold.Any())
                .Select(u => new ExportUserT8DTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = (int)u.Age,
                    SoldProducts = new ExportSoldProductsInfoDTO()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold
                        .Select(p => new ExportProductT8DTO()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .Take(10)
                .ToArray();

            ExportUsersInfoDTO usersInfoDTO = new ExportUsersInfoDTO()
            {
                Count = context.Users.Where(u => u.ProductsSold.Any()).Count(),
                Users = users
            };

            XmlSerializer serializer = new XmlSerializer(typeof(ExportUsersInfoDTO));

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, usersInfoDTO, xsn);
            }

            return builder.ToString().TrimEnd();
        }
    }
}
