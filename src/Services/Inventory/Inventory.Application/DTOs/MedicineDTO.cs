namespace Inventory.Application.DTOs
{
    public class MedicineDTO
    {
        public int Id { get; set; }
        public string? MedicineCode { get; set; } = string.Empty;
        public string? MedicineName { get; set; } = string.Empty;
        public string? Unit { get; set; } = string.Empty;
        public string? Manufacturer { get; set; } = string.Empty;
        public string? ActiveIngredient { get; set; } = string.Empty;
        public string? UsageInstructions { get; set; } = string.Empty;
        public string? Concentration { get; set; } = string.Empty;
        public string? Indications { get; set; } = string.Empty;
        public string? MedicineClassification { get; set; } = string.Empty;
        public string? RouteOfAdministration { get; set; } = string.Empty;
        public string? NationalMedicineCode { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; } = string.Empty;
        public int MedicineTypeId { get; set; }
        public int VaccineTypeCode { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
