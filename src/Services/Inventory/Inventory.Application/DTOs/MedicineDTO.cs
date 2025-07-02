namespace Inventory.Application.DTOs
{
    public class MedicineDTO
    {
        public int Id { get; set; }
        public string? MedicineCode { get; set; } 
        public string? MedicineName { get; set; } 
        public string? Unit { get; set; } 
        public string? ActiveIngredient { get; set; } 
        public string? UsageInstructions { get; set; } 
        public string? Concentration { get; set; } 
        public string? Indications { get; set; } 
        public string? MedicineClassification { get; set; } 
        public string? RouteOfAdministration { get; set; } 
        public string? NationalMedicineCode { get; set; } 
        public string? Description { get; set; } 
        public string? Note { get; set; } 
        public string? RegistrationNumber { get; set; } 
        public int MedicineTypeId { get; set; }
        public int VaccineTypeId { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
