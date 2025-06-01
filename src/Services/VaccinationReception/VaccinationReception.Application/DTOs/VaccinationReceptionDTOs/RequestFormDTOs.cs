using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationReceptionDTOs
{
    public record UnpaidServicesResponseDTO(
            List<UnpaidServiceDTO> Services,
            List<UnpaidVaccinationDTO> Vaccinations
        );

    public record UnpaidServiceDTO(
        int Id,
        string RequestNumber,
        int ServiceId,
        string ServiceName,
        int Quantity,
        decimal UnitPrice,
        DateTime CreatedAt
    );

    public record UnpaidVaccinationDTO(
        int Id,
        int VaccineId,
        int Quantity,
        DateTime CreatedAt
    );
}