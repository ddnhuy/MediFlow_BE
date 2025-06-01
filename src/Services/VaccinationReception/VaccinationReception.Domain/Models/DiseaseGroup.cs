using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class DiseaseGroup : BaseEntity
    {
        public string GroupName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<DiseaseGroupService> DiseaseGroupServices { get; set; } = new List<DiseaseGroupService>();
    }
}