namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            DateTime givenDate = DateTime.Parse(date);

            ExportPatientXmlDto[] patients = context.Patients
                .Where(p => p.PatientsMedicines.Any(m => m.Medicine.ProductionDate > givenDate))
                .Select(p => new ExportPatientXmlDto()
                {
                    Gender = p.Gender.ToString().ToLower(),
                    Name = p.FullName,
                    AgeGroup = p.AgeGroup.ToString(),
                    Medicines = p.PatientsMedicines
                    .Where(m => m.Medicine.ProductionDate > givenDate)
                    .OrderByDescending(m => m.Medicine.ExpiryDate)
                    .ThenBy(m => m.Medicine.Price)
                    .Select(m => new ExportMedicineXmlDto()
                    {
                        Category = m.Medicine.Category.ToString().ToLower(),
                        Name = m.Medicine.Name,
                        Price = m.Medicine.Price.ToString("F2"),
                        Producer = m.Medicine.Producer,
                        BestBefore = m.Medicine.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    })
                    .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(p => p.Name)
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Patients");

            XmlSerializer serializer = new XmlSerializer(typeof(ExportPatientXmlDto[]), xmlRoot);

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder builder = new StringBuilder();

            using (StringWriter writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, patients, xsn);
            }

            return builder.ToString().TrimEnd();
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            ExportMedicineJson[] medicines = context.Medicines
                .Where(m => (int)m.Category == medicineCategory && m.Pharmacy.IsNonStop)
                .ToArray()
                .Select(m => new ExportMedicineJson()
                {
                    Name = m.Name,
                    Price = m.Price.ToString("F2"),
                    Pharmacy = new ExportPharmacyJson()
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .OrderBy(m => m.Price)
                .ThenBy(m => m.Name)
                .ToArray();

            string result = JsonConvert.SerializeObject(medicines, Formatting.Indented);
            return result;
        }
    }
}
