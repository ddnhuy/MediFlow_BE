using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record UpdateReceptionVaccinationCommand(
            int Id,
            int ReceptionId,
            int Quantity,
            bool IsReadyToUse,
            DateTime ScheduledDate,
            DateTime? AppointmentDate,
            string? Note
        ) : ICommand<UpdateReceptionVaccinationResult>;

    public record UpdateReceptionVaccinationResult(bool IsSuccess);
}