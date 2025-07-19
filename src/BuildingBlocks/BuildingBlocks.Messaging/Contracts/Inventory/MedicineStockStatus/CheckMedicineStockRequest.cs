namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineStockStatus
{
    public class CheckMedicineStockRequest
    {
        public int MedicineId { get; init; }
        public decimal NumberOfMedicineWanted { get; init; }
        public string? RequestId { get; init; } = Guid.NewGuid().ToString();
        public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    }

    public class CheckMedicineStockResponse
    {
        public int MedicineId { get; init; }
        public decimal NumberOfMedicineWanted { get; init; }
        public decimal CurrentStock { get; init; }
        public bool IsEnough { get; init; }
        public decimal Difference { get; init; } // Positive if enough, negative if not
        public string? RequestId { get; init; }
        public DateTime RespondedAt { get; init; } = DateTime.UtcNow;
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
