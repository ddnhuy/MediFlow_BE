namespace Inventory.Domain.Models
{
    public class Supplier : Entity
    {
        public string? SupplierCode { get; set; } = string.Empty;
        public string? SupplierName { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Fax { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? TaxCode { get; set; } = string.Empty;
        public string? Director { get; set; } = string.Empty;
        public string? ContactPerson { get; set; } = string.Empty;
        public DateOnly ExpiredDate { get; set; }
    }
}
