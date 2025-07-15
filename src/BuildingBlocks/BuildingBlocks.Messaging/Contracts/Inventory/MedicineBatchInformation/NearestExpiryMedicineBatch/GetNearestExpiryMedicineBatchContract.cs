namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch
{
    public record GetNearestExpiryMedicineBatchRequest
    {
        public int MedicineId { get; init; }
        public string? RequestId { get; init; } = Guid.NewGuid().ToString();
        public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    }

    public record GetNearestExpiryMedicineBatchItem
    {
        public int MedicineBatchId { get; init; }
        public string? MedicineBatchNumber { get; init; }
        public int MedicineId { get; init; }
        public string? MedicineName { get; init; }
        public DateOnly? ExpiryDate { get; init; }
    }

    public record GetNearestExpiryMedicineBatchResponse
    {
        public List<GetNearestExpiryMedicineBatchItem> MedicineBatches { get; init; } = new List<GetNearestExpiryMedicineBatchItem>();
        public string? RequestId { get; init; }
        public DateTime RequestedAt { get; init; }
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
