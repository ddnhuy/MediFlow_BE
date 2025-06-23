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

            var command = context.Message.Adapt<CreateAppointmentCommand>();

            await sender.Send(command, context.CancellationToken);
        }
    }
}
