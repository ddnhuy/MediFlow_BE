using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public class CreateReceptionDTO
    {
        public int PatientId { get; set; }
        public DateTime ReceptionDate { get; set; }
        public int ServiceTypeId { get; set; }
    }
}