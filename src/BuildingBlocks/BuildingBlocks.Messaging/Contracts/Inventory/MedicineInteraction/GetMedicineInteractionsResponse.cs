namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction
{
    public class MedicineInteractionInfo
    {
        public int Id { get; init; }
        public int MedicineId1 { get; init; }
        public int MedicineId2 { get; init; }
        public string? Medicine1Name { get; init; }
        public string? Medicine2Name { get; init; }
        public string? HarmfulEffects { get; init; }
        public string? Mechanism { get; init; }
        public string? PreventiveActions { get; init; }
        public string? ReferenceInfo { get; init; }
        public string? Notes { get; init; }
    }

    public class GetMedicineInteractionsResponse
    {
        public int MedicineId { get; init; }
        public string? MedicineName { get; init; }
        public List<MedicineInteractionInfo> Interactions { get; init; } = new();
        public string? RequestId { get; init; }
        public DateTime RequestedAt { get; init; }
        public DateTime RespondedAt { get; init; } = DateTime.UtcNow;
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
    }
}