namespace BuildingBlocks.Messaging.Contracts.Inventory
{
    public class GetMedicineInformationRequest
    {
        public int MedicineId { get; init; }
        public string? RequestId { get; init; } = Guid.NewGuid().ToString();
        public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    }
}
