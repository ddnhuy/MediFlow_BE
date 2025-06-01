using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ServiceType : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public ICollection<Reception> Receptions { get; set; } = new List<Reception>();
    }
}