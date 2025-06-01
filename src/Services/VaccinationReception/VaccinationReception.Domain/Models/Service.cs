using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class Service : BaseEntity
    {
        public string ServiceCode { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int DepartmentId { get; set; }

        public ICollection<ServiceGroupService> ServiceGroupServices { get; set; } = new List<ServiceGroupService>();

        public ICollection<ServiceRequestDetail> ServiceRequestDetails { get; set; } = new List<ServiceRequestDetail>();
    }
}