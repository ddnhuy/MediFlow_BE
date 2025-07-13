namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction
{
    public class GetMedicineInteractionsRequest
    {
        public int MedicineId { get; init; }
        public string? RequestId { get; init; } = Guid.NewGuid().ToString();
        public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    }
}