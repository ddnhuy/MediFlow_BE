using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record AddServiceToRequestFormCommand(
        int ReceptionId,
        List<ServiceRequestItemDTO>? Services,
        string? GroupType,
        int? GroupId,
        int DefaultQuantity = 1
    ) : ICommand<AddServiceToRequestFormResult>;

    public record ServiceIdAndRequestNumberDTO(
        int ServiceId,
        string RequestNumber
    );
    public record AddServiceToRequestFormResult(
        int ReceptionId,
        List<ServiceIdAndRequestNumberDTO> ProcessedServiceReferences
    );
}