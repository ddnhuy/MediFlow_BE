using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.VaccinationDTOs;

namespace VaccinationReception.Application.DTOs.ExcelDTOs
{
    public class ContractVaccinationExcelDto
    {
        public string PatientName { get; set; } = string.Empty;
        public int Gender { get; set; }
        public DateTime DOB { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? IdentityCard { get; set; }
        public string? AddressDetail { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsForeigner { get; set; }
        public IList<VaccineDTO> Vaccines { get; set; } = new List<VaccineDTO>();

    }
}
