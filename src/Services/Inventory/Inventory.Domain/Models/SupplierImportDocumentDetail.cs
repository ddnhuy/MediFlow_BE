namespace Inventory.Domain.Models
{
    public class SupplierImportDocumentDetail : Entity
    {
        public int SupplierImportDocumentId { get; set; }
        public SupplierImportDocument? SupplierImportDocument { get; set; }
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public int MedicineBatchId { get; set; }
        public string? SGK_CPNK { get; set; }
        public string? Note { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public int ManufacturerId { get; set; }
        public int CountryId { get; set; }
        public bool IsFree { get; set; }
    }
}
