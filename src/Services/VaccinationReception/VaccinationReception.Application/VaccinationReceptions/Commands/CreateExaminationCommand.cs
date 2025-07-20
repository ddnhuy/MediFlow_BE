using BuildingBlocks.CQRS;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record CreateExaminationCommand (
        int ReceptionId, 
        int ServiceId,
        string RequestNumber,
        int PatientId,
        DateTime ReceptionTime
    ) : ICommand<CreateExaminationResult>;

    public record CreateExaminationResult(int ExaminationId);
}
