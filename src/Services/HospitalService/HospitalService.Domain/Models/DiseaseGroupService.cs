using HospitalService.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Domain.Models
{
    public class DiseaseGroupService : BaseEntity
    {
        public int DiseaseGroupId { get; set; }
        public DiseaseGroup DiseaseGroup { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
