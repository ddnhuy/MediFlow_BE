using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs
{
    public class ContractDTO
    {
        public string ContractCode { get; set; } = string.Empty;
        public int ContractNumber { get; set; }
        public string ContractName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public DateTime ContractDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? Description { get; set; }
        public Guid? FileContractId { get; set; }
        public Guid? FileVaccinationEnrollmentId { get; set; }
        public string? FileContractName { get; set; }
        public string? FileVaccinationEnrollmentName { get; set; }
    }
}
