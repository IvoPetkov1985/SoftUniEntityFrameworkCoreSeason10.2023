using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();

            string suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            string partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            string carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            string customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            string salesXml = File.ReadAllText("../../../Datasets/sales.xml");

            string taskResult = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(taskResult);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Suppliers");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportSupplierDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportSupplierDTO[]? supplierDTOs = (ImportSupplierDTO[]?)serializer.Deserialize(reader);

            ICollection<Supplier> suppliers = new List<Supplier>();

            if (supplierDTOs == null)
            {
                return $"Successfully imported 0";
            }

            foreach (ImportSupplierDTO supplierDTO in supplierDTOs)
            {
                Supplier supplier = new Supplier();
                supplier.Name = supplierDTO.Name;
                supplier.IsImporter = supplierDTO.IsImporter;

                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Parts");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportPartDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportPartDTO[]? partDTOs = (ImportPartDTO[]?)serializer.Deserialize(reader);

            ICollection<Part> parts = new List<Part>();

            int[] supplierIds = context.Suppliers.Select(s => s.Id).ToArray();

            if (partDTOs == null)
            {
                return $"Successfully imported 0";
            }

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

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Cars");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCarDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportCarDTO[]? carDTOs = (ImportCarDTO[]?)serializer.Deserialize(reader);

            ICollection<Car> cars = new List<Car>();

            int[] partIds = context.Parts.Select(p => p.Id).ToArray();

            if (carDTOs == null)
            {
                return $"Successfully imported 0";
            }

            foreach (ImportCarDTO carDTO in carDTOs)
            {
                Car car = new Car();
                car.Make = carDTO.Make;
                car.Model = carDTO.Model;
                car.TraveledDistance = carDTO.TraveledDistance;

                foreach (ImportPartId part in carDTO.Parts.DistinctBy(p => p.Id))
                {
                    if (!partIds.Contains(part.Id))
                    {
                        continue;
                    }

                    car.PartsCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId = part.Id
                    });
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Customers");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCustomerDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportCustomerDTO[]? customerDTOs = (ImportCustomerDTO[]?)serializer.Deserialize(reader);

            ICollection<Customer> customers = new List<Customer>();

            if (customerDTOs == null)
            {
                return $"Successfully imported 0";
            }

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

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xra = new XmlRootAttribute("Sales");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportSaleDTO[]), xra);

            using StringReader reader = new StringReader(inputXml);

            ImportSaleDTO[]? saleDTOs = (ImportSaleDTO[]?)serializer.Deserialize(reader);

            ICollection<Sale> sales = new List<Sale>();

            int[] carIds = context.Cars.Select(c => c.Id).ToArray();

            if (saleDTOs == null)
            {
                return $"Successfully imported 0";
            }

            foreach (ImportSaleDTO saleDTO in saleDTOs)
            {
                if (!carIds.Contains(saleDTO.CarId))
                {
                    continue;
                }

                Sale sale = new Sale();
                sale.CarId = saleDTO.CarId;
                sale.CustomerId = saleDTO.CustomerId;
                sale.Discount = saleDTO.Discount;

                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            ExportCarWithDistDTO[] carsWithDistance = context.Cars
                .AsNoTracking()
                .Where(c => c.TraveledDistance > 2_000_000)
                .Select(c => new ExportCarWithDistDTO()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("cars");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarWithDistDTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, carsWithDistance, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            ExportBmwCarDTO[] bmwCars = context.Cars
                .AsNoTracking()
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportBmwCarDTO()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            XmlRootAttribute root = new XmlRootAttribute("cars");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportBmwCarDTO[]), root);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, bmwCars, namespaces);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportLocSupplierDTO[] locSuppliers = context.Suppliers
                .AsNoTracking()
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportLocSupplierDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("suppliers");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportLocSupplierDTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, locSuppliers, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarWithPartsDTO[] cars = context.Cars
                .AsNoTracking()
                .Select(c => new ExportCarWithPartsDTO()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars
                    .Select(pc => new ExportPartDTO()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(pc => pc.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("cars");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarWithPartsDTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, cars, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportCustomerDTO[] customers = context.Customers
                .AsNoTracking()
                .Include(c => c.Sales)
                .ThenInclude(s => s.Car)
                .ThenInclude(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .ToArray()
                .Where(c => c.Sales.Any())
                .Select(c => new ExportCustomerDTO()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s =>
                        s.Car.PartsCars.Sum(pc =>
                            Math.Round(c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price, 2)
                        )
                    )
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("customers");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCustomerDTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, customers, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSaleDTO[] sales = context.Sales
                .AsNoTracking()
                .Select(s => new ExportSaleDTO()
                {
                    Car = new ExportSaleCarDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (100 - s.Discount) / 100)
                })
                .ToArray();

            XmlRootAttribute xra = new XmlRootAttribute("sales");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportSaleDTO[]), xra);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, sales, xsn);
            }

            return builder.ToString().TrimEnd();
        }
    }
}
