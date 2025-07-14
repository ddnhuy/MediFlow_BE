using BuildingBlocks.Strings.Enums;

namespace Inventory.Domain.Models
{
    public class Medicine : Entity
    {
        public string? MedicineCode { get; set; } = string.Empty;
        public string? MedicineName { get; set; } = string.Empty;
        public string? Unit { get; set; } = string.Empty;
        public string? ActiveIngredient { get; set; } = string.Empty;
        public string? UsageInstructions { get; set; } = string.Empty;
        public string? Concentration { get; set; } = string.Empty;
        public string? Indications { get; set; } = string.Empty;
        public string? MedicineClassification { get; set; } = string.Empty;
        public RouteOfAdministration? RouteOfAdministration { get; set; }
        public string? NationalMedicineCode { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; } = string.Empty;     
        public bool? IsRequiredTestingBeforeUse { get; set; } = false;

        public int? MedicineTypeId { get; set; }
        public MedicineType? MedicineType { get; set; } = default!;

        public int? VaccineTypeId { get; set; }
        public VaccineType? VaccineType { get; set; } = default!;
    }
}
