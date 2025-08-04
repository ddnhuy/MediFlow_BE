using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Abstractions;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Domain.Models
{
    public class Contract : BaseEntity
    {
        public string ContractCode { get; set; }
        public int ContractNumber { get; set; }
        public string ContractName { get; set; }
        public string CompanyName { get; set; }
        public string UnitName { get; set; }
        public ContractStatus Status { get; set; }
        public int ExpectedPatientCount { get; set; }
        public int ExpectedVaccineCount { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public decimal ContractValue { get; set; }
        public decimal? AdvanceAmount { get; set; }
        public decimal? ActualAmount { get; set; }
        public string? Description { get; set; }
        public Guid? FileContractId { get; set; }
        public Guid? FileVaccinationEnrollmentId { get; set; }

        public string? FileContractName { get; set; }
        public string? FileVaccinationEnrollmentName { get; set; }

        public ICollection<Reception> Receptions { get; set; } = new List<Reception>();
        public ICollection<ContractServiceDetail> ServiceDetails { get; set; } = new List<ContractServiceDetail>();
        public ICollection<ContractPatientVaccination> PlannedPatientVaccinations { get; set; } = new List<ContractPatientVaccination>();
        public ICollection<PaymentContract> PaymentContracts { get; set; } = new List<PaymentContract>();
    }
}
