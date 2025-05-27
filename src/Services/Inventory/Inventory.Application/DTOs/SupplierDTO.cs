namespace Inventory.Application.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string? SupplierCode { get; set; } 
        public string? SupplierName { get; set; } 
        public string? Address { get; set; } 
        public string? Phone { get; set; } 
        public string? Fax { get; set; } 
        public string? Email { get; set; } 
        public string? TaxCode { get; set; } 
        public string? Director { get; set; } 
        public string? ContactPerson { get; set; } 
        public string? NormalizedName { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
