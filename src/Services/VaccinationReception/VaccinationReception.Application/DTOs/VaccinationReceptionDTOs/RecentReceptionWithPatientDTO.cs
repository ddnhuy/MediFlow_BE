using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class RecentReceptionWithPatientDTO
    {
        public int ReceptionId { get; set; }
        public DateTime ReceptionDate { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public PatientSummaryDTO Patient { get; set; } = null!;
    }
}
