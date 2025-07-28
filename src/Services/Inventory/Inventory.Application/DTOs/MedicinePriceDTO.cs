namespace Inventory.Application.DTOs
{
    public class MedicinePriceDTO
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public string? Currency { get; set; }
        public double? VatRate { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? OriginalPriceAfterVat { get; set; }
        public decimal? OriginalPriceBeforeVat { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
