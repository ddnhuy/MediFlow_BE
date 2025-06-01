using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ServiceGroupService : BaseEntity
    {
        public int ServiceGroupId { get; set; }
        public ServiceGroup ServiceGroup { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}