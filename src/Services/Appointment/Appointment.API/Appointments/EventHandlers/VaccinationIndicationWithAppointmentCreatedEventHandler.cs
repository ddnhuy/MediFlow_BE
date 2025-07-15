using Appointment.API.Appointments.Commands;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Appointment.API.Appointments.EventHandlers
{
    public class VaccinationIndicationWithAppointmentCreatedEventHandler(
        ISender sender,
        ILogger<VaccinationIndicationWithAppointmentCreatedEventHandler> logger)
        : IConsumer<VaccinationIndicationWithAppointmentCreatedEvent>
    {
        public async Task Consume(ConsumeContext<VaccinationIndicationWithAppointmentCreatedEvent> context)
        {
            logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

            var command = new CreateAppointmentCommand(
                UserId: context.Message.UserId,
                PatientId: context.Message.PatientId,
                AppointmentDate: context.Message.AppointmentDate,
                AppointmentType: AppointmentType.Vaccination,
                PatientCode: context.Message.PatientCode,
                PatientFullName: context.Message.PatientFullName,
                PatientDOB: context.Message.PatientDOB,
                PatientEmail: context.Message.PatientEmail,
                PatientPhoneNumber: context.Message.PatientPhoneNumber,
                VaccineName: context.Message.VaccineName,
                Note: context.Message.Note,
                DoctorId: context.Message.DoctorId,
                VaccineId: context.Message.VaccineId,
                Dose: context.Message.Dose ?? string.Empty
            );
            
            await sender.Send(command, context.CancellationToken);
        }
    }
}
