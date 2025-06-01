using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class Reception : BaseEntity
    {
        public int PatientId { get; set; }
        public DateTime ReceptionDate { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;

        public ScreeningEvaluationReport? ScreeningEvaluationReport { get; set; }
        public ICollection<ReceptionVaccination> ReceptionVaccinations { get; set; } = new List<ReceptionVaccination>();

        public ICollection<RequestForm> RequestForms { get; set; } = new List<RequestForm>();
    }
}