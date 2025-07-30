using BuildingBlocks.Strings.Enums;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Data;

namespace InventoryService.FunctionalTests.Helpers
{
    public static class DatabaseSeeder
    {
        public static void SeedTestData(ApplicationDbContext dbContext)
        {
            // Add test medicine types if they don't exist
            if (!dbContext.MedicineTypes.Any())
            {
                var medicineTypes = new List<MedicineType>
                {
                    new MedicineType
                    {
                        Id = 1,
                        MedicineTypeName = "Analgesic",
                        IsSuspended = false,
                        IsCancelled = false
                    }
                };
                dbContext.MedicineTypes.AddRange(medicineTypes);
                dbContext.SaveChanges();
            }

            // Add test vaccine types if they don't exist
            if (!dbContext.VaccineTypes.Any())
            {
                var vaccineTypes = new List<VaccineType>
                {
                    new VaccineType
                    {
                        Id = 1,
                        VaccineTypeName = "Not Applicable",
                        IsSuspended = false,
                        IsCancelled = false
                    }
                };
                dbContext.VaccineTypes.AddRange(vaccineTypes);
                dbContext.SaveChanges();
            }

            if (!dbContext.Medicines.Any())
            {
                var medicines = new List<Medicine>
                {
                    new Medicine
                    {
                        MedicineCode = "VAC-001",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "MED001",
                        MedicineName = "Paracetamol",
                        Unit = "Tablet",
                        ActiveIngredient = "Paracetamol",
                        UsageInstructions = "Take 1-2 tablets every 4-6 hours",
                        Concentration = "500mg",
                        Indications = "Pain relief and fever reduction",
                        MedicineClassification = "Analgesic",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "23456-789-01",
                        Description = "Common pain reliever",
                        Note = "Take with food",
                        RegistrationNumber = "REG23456",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "MED002",
                        MedicineName = "Ibuprofen",
                        Unit = "Tablet",
                        ActiveIngredient = "Ibuprofen",
                        UsageInstructions = "Take 1-2 tablets every 6-8 hours",
                        Concentration = "400mg",
                        Indications = "Pain relief and anti-inflammatory",
                        MedicineClassification = "NSAID",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "34567-890-12",
                        Description = "Anti-inflammatory medication",
                        Note = "Take with food",
                        RegistrationNumber = "REG34567",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-002",
                        MedicineName = "Influenza Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "Inactivated Influenza Virus",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "15 mcg/0.5ml",
                        Indications = "Prevention of seasonal influenza",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "45678-901-23",
                        Description = "Seasonal influenza vaccine",
                        Note = "Store at 2-8°C",
                        RegistrationNumber = "REG45678",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {  
                        MedicineCode = "MED003",
                        MedicineName = "Aspirin",
                        Unit = "Tablet",
                        ActiveIngredient = "Acetylsalicylic Acid",
                        UsageInstructions = "Take 1 tablet daily",
                        Concentration = "100mg",
                        Indications = "Blood thinning and pain relief",
                        MedicineClassification = "Antiplatelet",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "56789-012-34",
                        Description = "Blood thinner medication",
                        Note = "Take with food",
                        RegistrationNumber = "REG56789",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "ISO123",
                        MedicineName = "Isomina Vaccine",
                        Unit = "ml",
                        ActiveIngredient = "Isomina Active",
                        UsageInstructions = "Inject once daily",
                        Concentration = "10mg/ml",
                        Indications = "Prevent Isomina virus",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "VN-ISO-001",
                        Description = "For testing use only",
                        Note = null,
                        RegistrationNumber = null,
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsRequiredTestingBeforeUse = true,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "ISO123",
                        MedicineName = "Isomina Vaccine",
                        Unit = "ml",
                        ActiveIngredient = "Isomina Active",
                        UsageInstructions = "Inject once daily",
                        Concentration = "10mg/ml",
                        Indications = "Prevent Isomina virus",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "VN-ISO-001",
                        Description = "For testing use only",
                        Note = null,
                        RegistrationNumber = null,
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsRequiredTestingBeforeUse = true,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "ISO123",
                        MedicineName = "Isomina Vaccine",
                        Unit = "ml",
                        ActiveIngredient = "Isomina Active",
                        UsageInstructions = "Inject once daily",
                        Concentration = "10mg/ml",
                        Indications = "Prevent Isomina virus",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "VN-ISO-001",
                        Description = "For testing use only",
                        Note = null,
                        RegistrationNumber = null,
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsRequiredTestingBeforeUse = true,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "ISO123",
                        MedicineName = "Isomina Vaccine",
                        Unit = "ml",
                        ActiveIngredient = "Isomina Active",
                        UsageInstructions = "Inject once daily",
                        Concentration = "10mg/ml",
                        Indications = "Prevent Isomina virus",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "VN-ISO-001",
                        Description = "For testing use only",
                        Note = null,
                        RegistrationNumber = null,
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsRequiredTestingBeforeUse = true,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-007",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-008",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-009",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-010",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-011",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    new Medicine
                    {
                        MedicineCode = "VAC-012",
                        MedicineName = "COVID-19 Vaccine",
                        Unit = "Dose",
                        ActiveIngredient = "mRNA-1273",
                        UsageInstructions = "Inject 0.5ml intramuscularly",
                        Concentration = "100 mcg/0.5ml",
                        Indications = "Prevention of COVID-19",
                        MedicineClassification = "Vaccine",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "12345-678-90",
                        Description = "COVID-19 mRNA Vaccine",
                        Note = "Store at -20°C",
                        RegistrationNumber = "REG12345",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    // Add a suspended medicine for testing
                    new Medicine
                    {
                        MedicineCode = "SUS001",
                        MedicineName = "Suspended Medicine",
                        Unit = "Tablet",
                        ActiveIngredient = "Test Ingredient",
                        UsageInstructions = "Test usage",
                        Concentration = "100mg",
                        Indications = "Test indications",
                        MedicineClassification = "Test",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "SUS-001",
                        Description = "Suspended medicine for testing",
                        Note = "This medicine is suspended",
                        RegistrationNumber = "REGSUS001",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = true,
                        IsCancelled = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    },
                    // Add a cancelled medicine for testing
                    new Medicine
                    {
                        MedicineCode = "CAN001",
                        MedicineName = "Cancelled Medicine",
                        Unit = "Tablet",
                        ActiveIngredient = "Test Ingredient",
                        UsageInstructions = "Test usage",
                        Concentration = "100mg",
                        Indications = "Test indications",
                        MedicineClassification = "Test",
                        RouteOfAdministration =  RouteOfAdministration.IM,
                        NationalMedicineCode = "CAN-001",
                        Description = "Cancelled medicine for testing",
                        Note = "This medicine is cancelled",
                        RegistrationNumber = "REGCAN001",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    }
                };
                dbContext.Medicines.AddRange(medicines);
                dbContext.SaveChanges();
            }

            // Add test medicine prices if they don't exist
            if (!dbContext.MedicinePrices.Any())
            {
                var now = DateTime.UtcNow;
                var medicinePrices = new List<MedicinePrice>
                {
                    // COVID-19 Vaccine
                    new MedicinePrice
                    {
                        MedicineId = 1,
                        UnitPrice = 625000m,
                        Currency = "VND",
                        VatRate = 0.05,
                        VatAmount = 31250m,
                        OriginalPriceBeforeVat = 625000m,
                        OriginalPriceAfterVat = 656250m,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = now,
                        CreatedBy = 1,
                        LastUpdatedAt = now,
                        LastUpdatedBy = 1
                    },
                    // Paracetamol
                    new MedicinePrice
                    {
                        MedicineId = 2,
                        UnitPrice = 5000m,
                        Currency = "VND",
                        VatRate = 0.05,
                        VatAmount = 250m,
                        OriginalPriceBeforeVat = 5000m,
                        OriginalPriceAfterVat = 5250m,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = now,
                        CreatedBy = 1,
                        LastUpdatedAt = now,
                        LastUpdatedBy = 1
                    },
                    // Ibuprofen
                    new MedicinePrice
                    {
                        MedicineId = 3,
                        UnitPrice = 7500m,
                        Currency = "VND",
                        VatRate = 0.05,
                        VatAmount = 375m,
                        OriginalPriceBeforeVat = 7500m,
                        OriginalPriceAfterVat = 7875m,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = now,
                        CreatedBy = 1,
                        LastUpdatedAt = now,
                        LastUpdatedBy = 1
                    },
                    // Influenza Vaccine
                    new MedicinePrice
                    {
                        MedicineId = 4,
                        UnitPrice = 387500m,
                        Currency = "VND",
                        VatRate = 0.05,
                        VatAmount = 19375m,
                        OriginalPriceBeforeVat = 387500m,
                        OriginalPriceAfterVat = 406875m,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = now,
                        CreatedBy = 1,
                        LastUpdatedAt = now,
                        LastUpdatedBy = 1
                    },
                    // Aspirin
                    new MedicinePrice
                    {
                        MedicineId = 5,
                        UnitPrice = 3000m,
                        Currency = "VND",
                        VatRate = 0.05,
                        VatAmount = 150m,
                        OriginalPriceBeforeVat = 3000m,
                        OriginalPriceAfterVat = 3150m,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = now,
                        CreatedBy = 1,
                        LastUpdatedAt = now,
                        LastUpdatedBy = 1
                    },
                    // Isomina Vaccine (no price - for testing null price scenario)
                    // Note: Medicine 6 (Isomina Vaccine) intentionally has no price                  
                };
                dbContext.MedicinePrices.AddRange(medicinePrices);
                dbContext.SaveChanges();
            }

            // Add test interactions if they don't exist
            if (!dbContext.MedicineInteractions.Any())
            {
                var interactions = new List<MedicineInteraction>
                {
                    new MedicineInteraction
                    {
                        MedicineId1 = 1,
                        MedicineId2 = 2,
                        HarmfulEffects = "Test harmful effects",
                        Mechanism = "Test mechanism",
                        PreventiveActions = "Test preventive actions",
                        ReferenceInfo = "Test reference",
                        Notes = "Test notes",
                        IsSuspended = false,
                        IsCancelled = false,
                    },
                    new MedicineInteraction
                    {
                        MedicineId1 = 2,
                        MedicineId2 = 3,
                        HarmfulEffects = "Test harmful effects 2",
                        Mechanism = "Test mechanism 2",
                        PreventiveActions = "Test preventive actions 2",
                        ReferenceInfo = "Test reference 2",
                        Notes = "Test notes 2",
                        IsSuspended = false,
                        IsCancelled = false,
                    }
                };
                dbContext.MedicineInteractions.AddRange(interactions);
                dbContext.SaveChanges();
            }

            // Add test warehouse types if they don't exist
            if (!dbContext.WarehouseTypes.Any())
            {
                var warehouseTypes = new List<WarehouseType>
                {
                    new WarehouseType
                    {
                        Id = 1,
                        WarehouseTypeCode = "HOSP",
                        WarehouseTypeName = "Hospital",
                        IsSuspended = false,
                        IsCancelled = false
                    },
                    new WarehouseType
                    {
                        Id = 2,
                        WarehouseTypeCode = "PHARM",
                        WarehouseTypeName = "Pharmacy",
                        IsSuspended = false,
                        IsCancelled = false
                    }
                };
                dbContext.WarehouseTypes.AddRange(warehouseTypes);
                dbContext.SaveChanges();
            }

            // Add test warehouses if they don't exist
            if (!dbContext.Warehouses.Any())
            {
                var warehouses = new List<Warehouse>
                {
                    new Warehouse
                    {
                        Id = 1,
                        WarehouseCode = "WH001",
                        WarehouseName = "Main Hospital Storage",
                        WarehouseTypeId = 1, // Hospital type
                        IsSuspended = false,
                        IsCancelled = false
                    },
                    new Warehouse
                    {
                        Id = 2,
                        WarehouseCode = "WH002",
                        WarehouseName = "Central Pharmacy",
                        WarehouseTypeId = 2, // Pharmacy type
                        IsSuspended = false,
                        IsCancelled = false
                    }
                };
                dbContext.Warehouses.AddRange(warehouses);
                dbContext.SaveChanges();
            }

            // Add test suppliers if they don't exist
            if (!dbContext.Suppliers.Any())
            {
                var suppliers = new List<Supplier>
                {
                    new Supplier
                    {
                        SupplierCode = "SUP001",
                        SupplierName = "MedPharm Supply Co.",
                        Address = "123 Medical Plaza, Suite 100",
                        Phone = "555-123-4567",
                        Fax = "555-123-4568",
                        Email = "info@medpharm.example",
                        TaxCode = "MP12345",
                        Director = "Sarah Johnson",
                        ContactPerson = "Michael Lewis",
                        IsSuspended = false,
                        IsCancelled = false
                    },
                    new Supplier
                    {
                        SupplierCode = "SUP002",
                        SupplierName = "Healthcare Distributors Inc.",
                        Address = "456 Hospital Drive",
                        Phone = "555-987-6543",
                        Fax = "555-987-6544",
                        Email = "sales@healthdist.example",
                        TaxCode = "HD67890",
                        Director = "Robert Chen",
                        ContactPerson = "Emma Wilson",
                        IsSuspended = false,
                        IsCancelled = false
                    },
                    new Supplier
                    {
                        SupplierCode = "SUP003",
                        SupplierName = "Global Meds Ltd.",
                        Address = "789 Pharmacy Road",
                        Phone = "555-246-8135",
                        Fax = "555-246-8136",
                        Email = "contact@globalmeds.example",
                        TaxCode = "GM24680",
                        Director = "James Taylor",
                        ContactPerson = "Olivia Martinez",
                        IsSuspended = false,
                        IsCancelled = false
                    }
                };
                dbContext.Suppliers.AddRange(suppliers);
                dbContext.SaveChanges();
            }

            // Check if manufacturer with ID 1 exists, if not add it
            if (!dbContext.Manufacturers.Any(m => m.Id == 1))
            {
                dbContext.Manufacturers.Add(new Manufacturer
                {
                    Id = 1,
                    ManufacturerName = "Test Manufacturer",
                    IsCancelled = false,
                    IsSuspended = false
                });
                dbContext.SaveChanges();
            }

            // Check if country with ID 1 exists, if not add it
            if (!dbContext.Countries.Any(c => c.Id == 1))
            {
                dbContext.Countries.Add(new Country
                {
                    Id = 1,
                    CountryName = "Test Country",
                    IsSuspended = false,
                    IsCancelled = false
                });
                dbContext.SaveChanges();
            }
        }
    }
}