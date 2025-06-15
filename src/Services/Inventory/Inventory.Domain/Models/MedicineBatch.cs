namespace Inventory.Domain.Models
{
    public class MedicineBatch : Entity
    {
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public string BatchNumber { get; set; } = default!;
        public DateOnly ImportDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public int? ManufacturerId { get; set; }
        public Manufacturer? Manufacturer { get; set; }
        // TODO: Other properties about Visa and import permit
    }
}
