using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();

            string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            string salesJson = File.ReadAllText("../../../Datasets/sales.json");

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ImportSupplierDTO[] supplierDTOs = JsonConvert.DeserializeObject<ImportSupplierDTO[]>(inputJson);

            ICollection<Supplier> suppliers = new List<Supplier>();

            foreach (ImportSupplierDTO supplierDTO in supplierDTOs)
            {
                Supplier supplier = new Supplier();
                supplier.Name = supplierDTO.Name;
                supplier.IsImporter = supplierDTO.IsImporter;

                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ImportPartDTO[] partDTOs = JsonConvert.DeserializeObject<ImportPartDTO[]>(inputJson);

            ICollection<Part> parts = new List<Part>();

            IEnumerable<int> supplierIds = context.Suppliers.Select(s => s.Id);

            foreach (ImportPartDTO partDTO in partDTOs)
            {
                if (!supplierIds.Contains(partDTO.SupplierId))
                {
                    continue;
                }

                Part part = new Part();
                part.Name = partDTO.Name;
                part.Price = partDTO.Price;
                part.Quantity = partDTO.Quantity;
                part.SupplierId = partDTO.SupplierId;

                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportCarDTO[] carDTOs = JsonConvert.DeserializeObject<ImportCarDTO[]>(inputJson);

            ICollection<Car> cars = new List<Car>();

            foreach (ImportCarDTO carDTO in carDTOs)
            {
                Car car = new Car();
                car.Make = carDTO.Make;
                car.Model = carDTO.Model;
                car.TraveledDistance = carDTO.TraveledDistance;

                foreach (int partId in carDTO.Parts.Distinct())
                {
                    car.PartsCars.Add(new PartCar()
                    {
                        PartId = partId,
                        Car = car
                    });
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            ImportCustomerDTO[] customerDTOs = JsonConvert.DeserializeObject<ImportCustomerDTO[]>(inputJson);

            ICollection<Customer> customers = new List<Customer>();

            foreach (ImportCustomerDTO customerDTO in customerDTOs)
            {
                Customer customer = new Customer();
                customer.Name = customerDTO.Name;
                customer.BirthDate = customerDTO.BirthDate;
                customer.IsYoungDriver = customerDTO.IsYoungDriver;

                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IEnumerable<ImportSaleDTO> saleDTOs = JsonConvert.DeserializeObject<IEnumerable<ImportSaleDTO>>(inputJson);

            ICollection<Sale> sales = new List<Sale>();

            foreach (ImportSaleDTO saleDTO in saleDTOs)
            {
                Sale sale = new Sale();
                sale.CarId = saleDTO.CarId;
                sale.CustomerId = saleDTO.CustomerId;
                sale.Discount = saleDTO.Discount;

                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            ExportCustomerDTO[] customers = context.Customers
                .AsNoTracking()
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new ExportCustomerDTO()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            IEnumerable<ExportToyotaCarDTO> toyotaCars = context.Cars
                .AsNoTracking()
                .Where(c => c.Make == "Toyota")
                .Select(c => new ExportToyotaCarDTO()
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .AsEnumerable();

            string result = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);
            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            IEnumerable<ExportLocSupplierDTO> localSuppliers = context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocSupplierDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .AsEnumerable();

            string result = JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            CarPartsDTO[] cars = context.Cars
                .AsNoTracking()
                .Select(c => new CarPartsDTO()
                {
                    Car = new ExportCarDTO()
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TraveledDistance = c.TraveledDistance
                    },
                    Parts = c.PartsCars
                    .Select(pc => new ExportPartDTO()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("F2")
                    })
                    .ToArray()
                })
                .ToArray();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportCustInfoDTO[] customers = context.Customers
                .AsNoTracking()
                .Include(c => c.Sales)
                .ThenInclude(s => s.Car)
                .ThenInclude(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .Where(c => c.Sales.Any())
                .ToArray()
                .Select(c => new ExportCustInfoDTO()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .ToArray();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSaleDTO[] sales = context.Sales
                .AsNoTracking()
                .Include(s => s.Car)
                .ThenInclude(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .Select(s => new ExportSaleDTO()
                {
                    Car = new ExportCarDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount.ToString("F2"),
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    PriceWithDiscount = Math.Round(s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100), 2).ToString("F2")
                })
                .Take(10)
                .ToArray();

            string result = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return result;
        }
    }
}
