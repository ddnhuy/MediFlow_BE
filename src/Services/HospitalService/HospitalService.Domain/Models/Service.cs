using HospitalService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Models
{
    public class Service : BaseEntity
    {
        public string ServiceCode { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int DepartmentId { get; set; }

        public virtual ICollection<ServiceGroupService> ServiceGroupServices { get; set; } = new List<ServiceGroupService>();
        public virtual ICollection<DiseaseGroupService> DiseaseGroupServices { get; set; } = new List<DiseaseGroupService>();
    }
}
