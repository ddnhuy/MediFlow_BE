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
            // Add test suppliers if they don't exist
            if (!dbContext.Suppliers.Any())
            {
                var suppliers = new List<Supplier>
                {
                    new Supplier
                    {
                        Id = 1,
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
                        Id = 2,
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
                        Id = 3,
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
        }
    }   
}

