using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;

namespace VaccinationReception.Application.DTOs.HospitalFeeDTOs
{
    public record PaymentWithPatientDTO(
        PaymentDTO Payment,
        PatientSummaryDTO Patient
    );
}
