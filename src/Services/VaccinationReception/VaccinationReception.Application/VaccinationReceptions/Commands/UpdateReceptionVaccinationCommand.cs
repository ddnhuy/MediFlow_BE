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
            int Quantity,
            bool IsReadyToUse,
            DateTime ScheduledDate,
            DateTime InvoiceDate,
            DateTime AppointmentDate,
            PaymentStatusForItem PaymentStatus,
            bool IsConfirmed,
            string? Note,
            string? TestResultEntry,
            int DoctorId
        ) : ICommand<UpdateReceptionVaccinationResult>;

    public record UpdateReceptionVaccinationResult(bool IsSuccess);
}