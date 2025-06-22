using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.HospitalFeeDTOs
{
    public record PatientPaymentsResponseDTO(
        int PatientId,
        List<PaymentDTO> Payments
    );
}
