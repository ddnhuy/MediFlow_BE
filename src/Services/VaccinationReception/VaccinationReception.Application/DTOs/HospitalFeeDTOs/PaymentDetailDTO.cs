using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.HospitalFeeDTOs
{
    public record PaymentDetailDTO(
        int Id,
        int? PaymentId,
        int? ReceptionVaccinationId,
        int? ServiceRequestDetailId,
        decimal Amount,
        bool IsReversed,
        DateTime CreatedAt,
        DateTime LastUpdatedAt,
        string? ServiceCode,
        string? ServiceName
    );
}
