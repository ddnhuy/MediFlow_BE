using BuildingBlocks.Strings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.HospitalService
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceName { get; set; }
        public decimal UnitPrice { get; set; }
        public int DepartmentId { get; set; }
        public int? Quantity { get; set; }
        public ExaminationService? ExaminationService { get; set; }
        public List<ServiceTestParameterDTO>? ServiceTestParameters { get; set; } = new List<ServiceTestParameterDTO>();
    }

    public class ServiceTestParameterDTO
    {
        public int ServiceId { get; set; }
        public string ParameterName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public string StandardValue { get; set; } = string.Empty;
        public string? EquipmentName { get; set; } = string.Empty;
        public string? SpecimenType { get; set; } = string.Empty;
    }
}
