namespace Inventory.Domain.Models
{
    public class MedicineBatchReturnDetail : Entity
    {
        public int MedicineBatchReturnId { get; set; }
        public MedicineBatchReturn? MedicineBatchReturn { get; set; }
        public int MedicineBatchId { get; set; }
        public string? BatchNumber { get; set; }
        public MedicineBatch? MedicineBatch { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public decimal Quantity { get; set; }
    }
}
