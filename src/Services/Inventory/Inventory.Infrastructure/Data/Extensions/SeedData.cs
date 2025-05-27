namespace Inventory.Infrastructure.Data.Extensions
{
    public static class SeedData
    {
        public static async Task<IApplicationBuilder> UseMigrationAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            // Seed the database with initial data if needed
            await SeedAsync(dbContext);
            return app;
        }

        private static Warehouse CreateNewWarehouse()
        {
            return new Warehouse
            {
                WarehouseCode = "WH-001",
                WarehouseName = "Main Warehouse",
                WarehouseTypeId = 1,
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
        }

        private static WarehouseType CreateNewWarehouseType()
        {
            return new WarehouseType
            {
                WarehouseTypeCode = "WH-001",
                WarehouseTypeName = "Medicine Warehouse",
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
        }

        // Add this method to the SeedData class
        private static List<Supplier> CreateNewSuppliers()
        {
            return new List<Supplier>
            {
                new Supplier
                {
                    SupplierCode = "SUP-001",
                    SupplierName = "MediPharma Supplies",
                    Address = "123 Medical Plaza, New York, NY 10001",
                    Phone = "0981995925",
                    Fax = "+1-212-555-0102",
                    Email = "contact@medipharma.com",
                    TaxCode = "MP12345678",
                    Director = "John Smith",
                    ContactPerson = "Sarah Johnson",
                    NormalizedName = "MEDIPHARMA SUPPLIES",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new Supplier
                {
                    SupplierCode = "SUP-002",
                    SupplierName = "VaccineWorld",
                    Address = "456 Immunization Drive, Boston, MA 02110",
                    Phone = "0981995925",
                    Fax = "+1-617-555-0202",
                    Email = "info@vaccineworld.com",
                    TaxCode = "VW87654321",
                    Director = "Emma Davis",
                    ContactPerson = "Michael Brown",
                    NormalizedName = "VACCINEWORLD",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new Supplier
                {
                    SupplierCode = "SUP-003",
                    SupplierName = "Global Medical Supplies",
                    Address = "789 Healthcare Blvd, Chicago, IL 60601",
                    Phone = "0981995925",
                    Fax = "+1-312-555-0302",
                    Email = "sales@globalmedical.com",
                    TaxCode = "GM24680135",
                    Director = "Robert Wilson",
                    ContactPerson = "Jennifer Lee",
                    NormalizedName = "GLOBAL MEDICAL SUPPLIES",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new Supplier
                {
                    SupplierCode = "SUP-004",
                    SupplierName = "PharmaTech Innovations",
                    Address = "101 Research Way, San Francisco, CA 94107",
                    Phone = "0981995925",
                    Fax = "+1-415-555-0402",
                    Email = "inquiries@pharmatech.com",
                    TaxCode = "PT13579246",
                    Director = "David Chen",
                    ContactPerson = "Amanda Rodriguez",
                    NormalizedName = "PHARMATECH INNOVATIONS",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new Supplier
                {
                    SupplierCode = "SUP-005",
                    SupplierName = "MediEquip Solutions",
                    Address = "202 Hospital Street, Seattle, WA 98101",
                    Phone = "0981995925",
                    Fax = "+1-206-555-0502",
                    Email = "support@mediequip.com",
                    TaxCode = "ME97531086",
                    Director = "Elizabeth Taylor",
                    ContactPerson = "Thomas Martin",
                    NormalizedName = "MEDIEQUIP SOLUTIONS",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                }
            };
        }

        private static MedicineType CreateNewMedicineType()
        {
            return new MedicineType
            {
                MedicineTypeCode = "MED-001",
                MedicineTypeName = "Prescription Medicine",
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
        }

        private static VaccineType CreateNewVaccineType()
        {
            return new VaccineType
            {
                VaccineTypeCode = "VAC-001",
                VaccineTypeName = "Inactivated Vaccine",
                Note = "Contains killed virus particles that cannot cause disease",
                IsSuspended = false,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
        }

        private static List<Medicine> CreateNewMedicines()
        {
            return new List<Medicine>
            {
                new Medicine
        {
            MedicineCode = "VAC-001",
            MedicineName = "COVID-19 Vaccine",
            Unit = "Dose",
            Manufacturer = "MediPharma",
            ActiveIngredient = "mRNA-1273",
            UsageInstructions = "Inject 0.5ml intramuscularly",
            Concentration = "100 mcg/0.5ml",
            Indications = "Prevention of COVID-19",
            MedicineClassification = "Vaccine",
            RouteOfAdministration = "Intramuscular",
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
                    MedicineCode = "VAC-002",
                    MedicineName = "Influenza Vaccine",
                    Unit = "Dose",
                    Manufacturer = "FluShield",
                    ActiveIngredient = "Inactivated Influenza Virus",
                    UsageInstructions = "Inject 0.5ml intramuscularly",
                    Concentration = "15 mcg/0.5ml",
                    Indications = "Prevention of seasonal influenza",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Intramuscular",
                    NationalMedicineCode = "23456-789-01",
                    Description = "Seasonal influenza vaccine",
                    Note = "Store at 2-8°C",
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
                    MedicineCode = "VAC-003",
                    MedicineName = "Hepatitis B Vaccine",
                    Unit = "Dose",
                    Manufacturer = "HepGuard",
                    ActiveIngredient = "Hepatitis B Surface Antigen",
                    UsageInstructions = "Inject 1.0ml intramuscularly",
                    Concentration = "20 mcg/ml",
                    Indications = "Prevention of Hepatitis B infection",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Intramuscular",
                    NationalMedicineCode = "34567-890-12",
                    Description = "Recombinant Hepatitis B vaccine",
                    Note = "Store at 2-8°C. Do not freeze.",
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
                    MedicineCode = "VAC-004",
                    MedicineName = "Pneumococcal Vaccine",
                    Unit = "Dose",
                    Manufacturer = "LungDefense",
                    ActiveIngredient = "Purified Capsular Polysaccharides",
                    UsageInstructions = "Inject 0.5ml intramuscularly",
                    Concentration = "25 mcg/0.5ml",
                    Indications = "Prevention of pneumococcal disease",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Intramuscular",
                    NationalMedicineCode = "45678-901-23",
                    Description = "23-valent pneumococcal polysaccharide vaccine",
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
                    MedicineCode = "VAC-005",
                    MedicineName = "MMR Vaccine",
                    Unit = "Dose",
                    Manufacturer = "TriShield",
                    ActiveIngredient = "Live Attenuated Measles, Mumps, and Rubella Viruses",
                    UsageInstructions = "Inject 0.5ml subcutaneously",
                    Concentration = "1000 TCID50/0.5ml",
                    Indications = "Prevention of measles, mumps, and rubella",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Subcutaneous",
                    NationalMedicineCode = "56789-012-34",
                    Description = "Combined measles, mumps, and rubella vaccine",
                    Note = "Store at -15°C or colder",
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
                    MedicineCode = "VAC-006",
                    MedicineName = "Tdap Vaccine",
                    Unit = "Dose",
                    Manufacturer = "ImmunePlus",
                    ActiveIngredient = "Tetanus Toxoid, Diphtheria Toxoid, Acellular Pertussis",
                    UsageInstructions = "Inject 0.5ml intramuscularly",
                    Concentration = "5 Lf/0.5ml",
                    Indications = "Prevention of tetanus, diphtheria, and pertussis",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Intramuscular",
                    NationalMedicineCode = "67890-123-45",
                    Description = "Combined tetanus, diphtheria, and pertussis vaccine",
                    Note = "Store at 2-8°C",
                    RegistrationNumber = "REG67890",
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
                    MedicineCode = "VAC-007",
                    MedicineName = "Varicella Vaccine",
                    Unit = "Dose",
                    Manufacturer = "PoxGuard",
                    ActiveIngredient = "Live Attenuated Varicella-Zoster Virus",
                    UsageInstructions = "Inject 0.5ml subcutaneously",
                    Concentration = "1350 PFU/0.5ml",
                    Indications = "Prevention of chickenpox",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Subcutaneous",
                    NationalMedicineCode = "78901-234-56",
                    Description = "Chickenpox vaccine",
                    Note = "Store frozen at -15°C or colder",
                    RegistrationNumber = "REG78901",
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
                    MedicineName = "HPV Vaccine",
                    Unit = "Dose",
                    Manufacturer = "CancerShield",
                    ActiveIngredient = "Human Papillomavirus Types 6, 11, 16, 18, 31, 33, 45, 52, 58 L1 VLPs",
                    UsageInstructions = "Inject 0.5ml intramuscularly",
                    Concentration = "30 mcg/0.5ml",
                    Indications = "Prevention of HPV-related cancers and genital warts",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Intramuscular",
                    NationalMedicineCode = "89012-345-67",
                    Description = "9-valent human papillomavirus vaccine",
                    Note = "Store at 2-8°C",
                    RegistrationNumber = "REG89012",
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
                    MedicineName = "Polio Vaccine (IPV)",
                    Unit = "Dose",
                    Manufacturer = "PolioDefend",
                    ActiveIngredient = "Inactivated Poliovirus Types 1, 2, and 3",
                    UsageInstructions = "Inject 0.5ml subcutaneously or intramuscularly",
                    Concentration = "40 D-antigen units/0.5ml",
                    Indications = "Prevention of poliomyelitis",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Subcutaneous or Intramuscular",
                    NationalMedicineCode = "90123-456-78",
                    Description = "Inactivated polio vaccine",
                    Note = "Store at 2-8°C",
                    RegistrationNumber = "REG90123",
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
                    MedicineName = "Meningococcal Vaccine",
                    Unit = "Dose",
                    Manufacturer = "MeningoGuard",
                    ActiveIngredient = "Meningococcal Polysaccharides A, C, Y, W-135",
                    UsageInstructions = "Inject 0.5ml intramuscularly",
                    Concentration = "4 mcg/0.5ml",
                    Indications = "Prevention of meningococcal disease",
                    MedicineClassification = "Vaccine",
                    RouteOfAdministration = "Intramuscular",
                    NationalMedicineCode = "01234-567-89",
                    Description = "Quadrivalent meningococcal vaccine",
                    Note = "Store at 2-8°C",
                    RegistrationNumber = "REG01234",
                    MedicineTypeId = 1,
                    VaccineTypeId = 1,
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                }
    };
        }

        private static List<MedicineInteraction> CreateNewMedicineInteractions()
        {
            return new List<MedicineInteraction>
            {
                new MedicineInteraction
                {
                    MedicineId1 = 1, // COVID-19 Vaccine
                    MedicineId2 = 2, // Influenza Vaccine
                    HarmfulEffects = "Potential increased risk of inflammatory response",
                    Mechanism = "Overlapping immune system activation pathways",
                    PreventiveActions = "Separate administration by at least 14 days",
                    ReferenceInfo = "CDC Vaccine Administration Guidelines 2023",
                    Notes = "Recommendation may change as more data becomes available",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new MedicineInteraction
                {
                    MedicineId1 = 3, // Hepatitis B Vaccine
                    MedicineId2 = 5, // MMR Vaccine
                    HarmfulEffects = "Reduced efficacy of both vaccines",
                    Mechanism = "Interference with immune response generation",
                    PreventiveActions = "Administer at different injection sites if given simultaneously",
                    ReferenceInfo = "Immunization Action Coalition Guidelines",
                    Notes = "If possible, schedule at least 4 weeks apart for optimal immune response",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new MedicineInteraction
                {
                    MedicineId1 = 6, // Tdap Vaccine
                    MedicineId2 = 7, // Varicella Vaccine
                    HarmfulEffects = "Increased risk of local injection site reactions",
                    Mechanism = "Cumulative inflammatory response at administration sites",
                    PreventiveActions = "Use different limbs for administration",
                    ReferenceInfo = "National Immunization Program Technical Guide",
                    Notes = "Monitor patient for 30 minutes after administration",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new MedicineInteraction
                {
                    MedicineId1 = 8, // HPV Vaccine
                    MedicineId2 = 10, // Meningococcal Vaccine
                    HarmfulEffects = "Increased incidence of fever and malaise",
                    Mechanism = "Additive systemic inflammatory response",
                    PreventiveActions = "Prophylactic antipyretic administration may be considered",
                    ReferenceInfo = "Vaccine Safety Handbook, 5th Edition",
                    Notes = "Effects more common in adolescents than adults",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new MedicineInteraction
                {
                    MedicineId1 = 4, // Pneumococcal Vaccine
                    MedicineId2 = 9, // Polio Vaccine
                    HarmfulEffects = "Potential for diminished antibody response",
                    Mechanism = "Competition for immune system resources",
                    PreventiveActions = "Consider separating by 8 weeks in immunocompromised patients",
                    ReferenceInfo = "International Vaccine Safety Coalition Report 2022",
                    Notes = "Interaction is more theoretical than clinically proven",
                    IsSuspended = false,
                    IsCancelled = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                }
            };
        }

        private static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.WarehouseTypes.AnyAsync())
            {
                var warehouseType = CreateNewWarehouseType();
                await context.WarehouseTypes.AddAsync(warehouseType);
                await context.SaveChangesAsync();
            }
            if (!await context.Warehouses.AnyAsync())
            {
                var warehouse = CreateNewWarehouse();
                await context.Warehouses.AddAsync(warehouse);
                await context.SaveChangesAsync();
            }
            // Seed MedicineType
            if (!await context.MedicineTypes.AnyAsync())
            {
                var medicineType = CreateNewMedicineType();
                await context.MedicineTypes.AddAsync(medicineType);
                await context.SaveChangesAsync();
            }

            // Seed VaccineType
            if (!await context.VaccineTypes.AnyAsync())
            {
                var vaccineType = CreateNewVaccineType();
                await context.VaccineTypes.AddAsync(vaccineType);
                await context.SaveChangesAsync();
            }

            // Seed Medicines
            if (!await context.Medicines.AnyAsync())
            {
                var medicines = CreateNewMedicines();
                await context.Medicines.AddRangeAsync(medicines);
                await context.SaveChangesAsync();
            }

            // Seed MedicineInteractions
            if (!await context.MedicineInteractions.AnyAsync())
            {
                var interactions = CreateNewMedicineInteractions();
                await context.MedicineInteractions.AddRangeAsync(interactions);
                await context.SaveChangesAsync();
            }

            // Seed Suppliers
            if (!await context.Suppliers.AnyAsync())
            {
                var suppliers = CreateNewSuppliers();
                await context.Suppliers.AddRangeAsync(suppliers);
                await context.SaveChangesAsync();
            }
        }
    }
}