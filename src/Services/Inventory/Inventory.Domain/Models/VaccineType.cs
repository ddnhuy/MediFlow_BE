namespace Inventory.Domain.Models
{
    public class VaccineType : Entity
    {
        public string? VaccineTypeCode { get; set; } = string.Empty;
        public string? VaccineTypeName { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
    }
}
