namespace Inventory.Application.DTOs
{
    public class ExpiredMedicineBatchDto
    {
        public int MedicineId { get; set; } 
        public string MedicineCode { get; set; } = default!;
        public string MedicineName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public int MedicineBatchId { get; set; }
        public string BatchNumber { get; set; } = default!;
        public DateOnly ExpiryDate { get; set; }
        public decimal CurrentQuantity { get; set; }
    }
}
