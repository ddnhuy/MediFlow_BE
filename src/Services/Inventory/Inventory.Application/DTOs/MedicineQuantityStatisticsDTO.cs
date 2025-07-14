namespace Inventory.Application.DTOs
{
    public class MedicineQuantityStatisticsDto
    {
        public string MedicineCode { get; set; } = default!;
        public string MedicineName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public int NumberOfBatches { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
