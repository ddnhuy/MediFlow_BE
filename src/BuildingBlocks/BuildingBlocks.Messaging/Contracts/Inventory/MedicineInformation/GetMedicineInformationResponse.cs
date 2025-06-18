namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation
{
    public class GetMedicineInformationResponse
    {
        public int MedicineId { get; init; }
        public string? MedicineCode { get; init; }
        public string? MedicineName { get; init; }
        public string? Unit { get; init; }
        public string? ActiveIngredient { get; init; }
        public string? UsageInstructions { get; init; }
        public string? Concentration { get; init; }
        public string? Indications { get; init; }
        public string? MedicineClassification { get; init; }
        public string? RouteOfAdministration { get; init; }
        public string? NationalMedicineCode { get; init; }
        public string? Description { get; init; }
        public string? Note { get; init; }
        public string? RegistrationNumber { get; init; }
        public bool? IsRequiredTestingBeforeUse { get; init; }
        public int? MedicineTypeId { get; init; }
        public string? MedicineTypeName { get; init; }
        public int? VaccineTypeId { get; init; }
        public string? VaccineTypeName { get; init; }
        public decimal? UnitPrice { get; init; }
        public bool IsSuspended { get; init; }
        public bool IsCancelled { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime LastUpdatedAt { get; init; }
        public string? RequestId { get; init; }
        public DateTime RespondedAt { get; init; } = DateTime.UtcNow;
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
