namespace Inventory.Domain.Models
{
    public class MedicineType : Entity
    {
        public string? MedicineTypeCode { get; set; } = string.Empty;
        public string? MedicineTypeName { get; set; } = string.Empty;
    }
}
