namespace Inventory.Application.DTOs
{
    public class ExpiredMedicineBatchDto
    {
        public string MedicineCode { get; set; } = default!;
        public string MedicineName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public string BatchNumber { get; set; } = default!;
        public DateOnly ExpiryDate { get; set; }
        public decimal CurrentQuantity { get; set; }
    }
}
