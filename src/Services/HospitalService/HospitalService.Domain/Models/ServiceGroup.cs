using HospitalService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Models
{
    public class ServiceGroup : BaseEntity
    {
        public string GroupName { get; set; } = null!;

        public ICollection<ServiceGroupService> ServiceGroupServices { get; set; } = new List<ServiceGroupService>();
    }
}
