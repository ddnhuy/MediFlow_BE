using HospitalService.Domain.Abstractions;
using HospitalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HospitalService.Domain.Models
{
    public class ServiceTestParameter : BaseEntity
    {
        public int ServiceId { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
        public string ParameterName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public string StandardValue { get; set; } = string.Empty;
        public string? EquipmentName { get; set; } = string.Empty;
        public string? SpecimenType { get; set; } = string.Empty;
    }
}
