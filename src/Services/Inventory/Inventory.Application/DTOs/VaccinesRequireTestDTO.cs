using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.DTOs
{
    public class VaccinesRequireTestDTO
    {
        public int Id { get; set; }
        public string? MedicineName { get; set; } = string.Empty;
        public string? Concentration { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; } = string.Empty;
        public string? ActiveIngredient { get; set; } = string.Empty;
        public string? RouteOfAdministration { get; set; } = string.Empty;
        public string? Unit { get; set; } = string.Empty;
        public string? VaccineTypeCode { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public string? NationalMedicineCode { get; set; } = string.Empty;
        public string? MedicineClassification { get; set; } = string.Empty;
        public int? VaccineTypeId { get; set; }
        public string? Indications { get; set; } = string.Empty;
        public string? UsageInstructions { get; set; } = string.Empty;
        public string? MedicineCode { get; set; } = string.Empty;
        public string? VaccineTypeName { get; set; } = string.Empty;
    }
}
