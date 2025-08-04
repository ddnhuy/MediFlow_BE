using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Domain.Models
{
    public class ContractPatientVaccination : BaseEntity
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;
        public int PatientId { get; set; }
        public int VaccineId { get; set; }
        public int DoseNumber { get; set; }
        public int? Quantity { get; set; }
        public ContractPatientVaccinationStatus Status { get; set; }

        public int? ReceptionVaccinationId { get; set; }
        public ReceptionVaccination? ReceptionVaccination { get; set; }
    }
}
