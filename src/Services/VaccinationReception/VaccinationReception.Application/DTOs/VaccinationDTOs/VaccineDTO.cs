using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationDTOs
{
    public class VaccineDTO
    {
        public string VaccineCode { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int DoseNumber { get; set; }
    }
}
