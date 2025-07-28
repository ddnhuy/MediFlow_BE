using BuildingBlocks.Strings.Enums;
using HospitalService.Domain.Abstractions;

namespace HospitalService.Domain.Models
{
    public class Service : BaseEntity
    {
        public string ServiceCode { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public ExaminationService? ExaminationService { get; set; }
        public int DepartmentId { get; set; }
        public ServiceType? ServiceType { get; set; }
        public virtual ICollection<ServiceGroupService> ServiceGroupServices { get; set; } = new List<ServiceGroupService>();
        public virtual ICollection<DiseaseGroupService> DiseaseGroupServices { get; set; } = new List<DiseaseGroupService>();
        public virtual ICollection<ServiceTestParameter> ServiceTestParameters { get; set; } = new List<ServiceTestParameter>();
    }
}
