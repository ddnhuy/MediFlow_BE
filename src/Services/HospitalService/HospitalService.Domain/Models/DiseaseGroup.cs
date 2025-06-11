using HospitalService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Models
{
    public class DiseaseGroup : BaseEntity
    {
        public string GroupName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<DiseaseGroupService> DiseaseGroupServices { get; set; } = new List<DiseaseGroupService>();
    }
}
