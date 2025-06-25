using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record CreateReceptionVaccinationCommand(
            int ReceptionId,
            int VaccineId,
            int Quantity,
            bool IsReadyToUse,
            DateTime ScheduledDate,
            DateTime InvoiceDate,
            DateTime AppointmentDate,
            PaymentStatusForItem PaymentStatusForItem,
            bool IsConfirmed,
            string? Note,
            string? TestResultEntry,
            int DoctorId
        ) : ICommand<CreateReceptionVaccinationResult>;

    public record CreateReceptionVaccinationResult(int ReceptionVaccinationId);
}