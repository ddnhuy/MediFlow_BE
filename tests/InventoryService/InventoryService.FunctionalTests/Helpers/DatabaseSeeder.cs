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

            // Add test medicines if they don't exist
            if (!dbContext.Medicines.Any())
            {
                var medicines = new List<Medicine>
                {
                    new Medicine
                    {
                        Id = 1,
                        MedicineName = "Paracetamol",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false
                    },
                    new Medicine
                    {
                        Id = 2,
                        MedicineName = "Ibuprofen",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false
                    },
                    new Medicine
                    {
                        Id = 3,
                        MedicineName = "Aspirin",
                        MedicineTypeId = 1,
                        VaccineTypeId = 1,
                        IsSuspended = false,
                        IsCancelled = false
                    },
                };
                dbContext.Medicines.AddRange(medicines);
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
        }
    }
}

