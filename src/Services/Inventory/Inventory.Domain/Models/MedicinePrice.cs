namespace Inventory.Domain.Models
{
    public class MedicinePrice : Entity
    {
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Currency { get; set; }
        public double VatRate { get; set; }
        public decimal VatAmount { get; set; }
        public decimal OriginalPriceAfterVat { get; set; }
        public decimal OriginalPriceBeforeVat { get; set; }
    }
}
