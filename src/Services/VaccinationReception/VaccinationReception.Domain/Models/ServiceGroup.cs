using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ServiceGroup : BaseEntity
    {
        public string GroupName { get; set; } = null!;

        public ICollection<ServiceGroupService> ServiceGroupServices { get; set; } = new List<ServiceGroupService>();
    }
}