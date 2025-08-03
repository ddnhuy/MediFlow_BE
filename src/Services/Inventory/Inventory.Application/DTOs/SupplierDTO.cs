namespace Inventory.Application.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string? SupplierCode { get; set; } 
        public string? SupplierName { get; set; } 
        public string? Address { get; set; } 
        public string? Phone { get; set; } 
        public string? Email { get; set; } 
        public string? ContactPerson { get; set; }
        public DateOnly ExpiredDate { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }

    public class SupplierContractDTO
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; } = string.Empty;
    }

    public class SupplierDetailDTO
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
        public DateOnly ExpiredDate { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
        public List<SupplierContractDTO> Contracts { get; set; } = new List<SupplierContractDTO>();
    }
}
