namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            ImportPatientDto[] patientDtos = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);

            StringBuilder builder = new StringBuilder();

            List<Patient> patients = new List<Patient>();

            foreach (ImportPatientDto patientDto in patientDtos)
            {
                if (!IsValid(patientDto))
                {
                    builder.AppendLine(ErrorMessage);
                    continue;
                }

                Patient patient = new Patient();
                patient.FullName = patientDto.FullName;
                patient.AgeGroup = (AgeGroup)patientDto.AgeGroup;
                patient.Gender = (Gender)patientDto.Gender;

                foreach (int medicineId in patientDto.Medicines)
                {
                    if (patient.PatientsMedicines.Any(m => m.Medicine.Id == medicineId))
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Medicine medicine = context.Medicines.Find(medicineId);

                    patient.PatientsMedicines.Add(new PatientMedicine()
                    {
                        Medicine = medicine
                    });
                }

                patients.Add(patient);
                builder.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count));
            }

            context.Patients.AddRange(patients);
            context.SaveChanges();

            return builder.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportPharmacyDto[]), new XmlRootAttribute("Pharmacies"));

            using StringReader reader = new StringReader(xmlString);

            ImportPharmacyDto[] pharmacyDtos = (ImportPharmacyDto[])serializer.Deserialize(reader);

            StringBuilder builder = new StringBuilder();

            List<Pharmacy> pharmacies = new List<Pharmacy>();

            foreach (ImportPharmacyDto pharmacyDto in pharmacyDtos)
            {
                if (!IsValid(pharmacyDto))
                {
                    builder.AppendLine(ErrorMessage);
                    continue;
                }

                bool parsedNonStopStatus;

                bool isIsNonStopValid = Boolean.TryParse(pharmacyDto.IsNonStop, out parsedNonStopStatus);

                if (!isIsNonStopValid)
                {
                    builder.AppendLine(ErrorMessage);
                    continue;
                }

                Pharmacy pharmacy = new Pharmacy();
                pharmacy.IsNonStop = parsedNonStopStatus;
                pharmacy.Name = pharmacyDto.Name;
                pharmacy.PhoneNumber = pharmacyDto.PhoneNumber;

                foreach (ImportMedicineDto medicineDto in pharmacyDto.Medicines)
                {
                    if (!IsValid(medicineDto))
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (string.IsNullOrEmpty(medicineDto.Producer))
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime? productionDate = null;

                    if (!string.IsNullOrEmpty(medicineDto.ProductionDate))
                    {
                        DateTime productionDateValue;

                        bool isProductionDateValid = DateTime.TryParseExact(medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out productionDateValue);

                        if (!isProductionDateValid)
                        {
                            builder.AppendLine(ErrorMessage);
                            continue;
                        }

                        productionDate = productionDateValue;
                    }
                    else
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime? expiryDate = null;

                    if (!string.IsNullOrEmpty(medicineDto.ExpiryDate))
                    {
                        DateTime expiryDateValue;

                        bool isExpiryDateValid = DateTime.TryParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiryDateValue);

                        if (!isExpiryDateValid)
                        {
                            builder.AppendLine(ErrorMessage);
                            continue;
                        }

                        expiryDate = expiryDateValue;
                    }
                    else
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (productionDate >= expiryDate)
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Medicine medicine = new Medicine();
                    medicine.Category = (Category)medicineDto.Category;
                    medicine.Name = medicineDto.Name;
                    medicine.Price = medicineDto.Price;
                    medicine.ProductionDate = (DateTime)productionDate;
                    medicine.ExpiryDate = (DateTime)expiryDate;
                    medicine.Producer = medicineDto.Producer;

                    if (pharmacy.Medicines.Any(m => m.Name == medicine.Name && m.Producer == medicine.Producer))
                    {
                        builder.AppendLine(ErrorMessage);
                        continue;
                    }

                    pharmacy.Medicines.Add(medicine);
                }

                pharmacies.Add(pharmacy);
                builder.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count));
            }

            context.Pharmacies.AddRange(pharmacies);
            context.SaveChanges();

            return builder.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
