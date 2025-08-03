using BuildingBlocks.Strings.Enums;

namespace Inventory.Domain.Models
{
    public class MedicineBatchReturn : Entity
    {
        public string ReturnCode { get; set; } = default!;
        public string? Reason { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverEmail { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? ApprovalToken { get; set; }
        public MedicineBatchReturnStatus Status { get; set; } = MedicineBatchReturnStatus.Pending;
    }
}
