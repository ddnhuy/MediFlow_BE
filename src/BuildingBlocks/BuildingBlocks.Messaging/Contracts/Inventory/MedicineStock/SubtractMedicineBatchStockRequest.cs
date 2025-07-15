using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineStock
{
    public class SubtractMedicineBatchStockRequest
    {
        public int MedicineBatchId { get; init; }
        public int Quantity { get; init; }
        public string? RequestId { get; init; } = Guid.NewGuid().ToString();
        public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    }

    public class SubtractMedicineBatchStockResponse
    {
        public int MedicineBatchId { get; init; }
        public int Quantity { get; init; }
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
        public string? RequestId { get; init; }
        public DateTime RespondedAt { get; init; } = DateTime.UtcNow;
    }
}
