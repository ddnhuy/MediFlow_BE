using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs
{
    public class ContractResponse
    {
        public int Id { get; set; }
        public string ContractCode { get; set; } = string.Empty;
        public int ContractNumber { get; set; }
        public string ContractName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public ContractStatus Status { get; set; }

        public int ExpectedPatientCount { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public decimal ContractValue { get; set; }
        public decimal? AdvanceAmount { get; set; }
        public decimal? ActualAmount { get; set; }
        public string? Description { get; set; }

        public Guid? FileContractId { get; set; }
        public string? FileContractName { get; set; }

        public Guid? FileVaccinationEnrollmentId { get; set; }
        public string? FileVaccinationEnrollmentName { get; set; }
    }
}
