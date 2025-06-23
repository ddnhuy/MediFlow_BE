using BuildingBlocks.DomainEvents;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Strings.Enums;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstractions.CurrentUser;
using VaccinationReception.Application.Services.PatientServices;


namespace VaccinationReception.Application.VaccinationReceptions.EventHandlers
{
    internal class ReceptionVaccinationCreatedEvent : IDomainEvent
    {
        public int PatientId { get; set; }
        public int VaccineId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Note { get; set; }
    }

    internal class ReceptionVaccinationCreatedEventHandler(
        ILogger<ReceptionVaccinationCreatedEventHandler> logger,
        ICurrentUserHelper currentUserHelper,
        IPatientGrpcClient patientGrpcClient,
        IPublishEndpoint publishEndpoint) : INotificationHandler<ReceptionVaccinationCreatedEvent>
    {
        public async Task Handle(ReceptionVaccinationCreatedEvent notification, CancellationToken cancellationToken)
        {
            var getPatient = patientGrpcClient.GetPatientAsync(notification.PatientId, cancellationToken);
            // var getVaccine = patientGrpcClient.GetVaccineAsync(notification.VaccineId, cancellationToken);

            var patient = await getPatient;

            var integrationEvent = new VaccinationIndicationWithAppointmentCreatedEvent
            {
                UserId = currentUserHelper.UserId,
                PatientId = notification.PatientId,
                AppointmentDate = notification.AppointmentDate.AddHours(-7).ToUniversalTime(),
                AppointmentType = AppointmentType.Vaccination,
                PatientCode = patient.Code,
                PatientFullName = patient.Name,
                PatientDOB = patient.DOB,
                PatientEmail = patient.Email ?? string.Empty,
                PatientPhoneNumber = patient.PhoneNumber,
                VaccineName = null, // Assuming this will be set later
                Note = notification.Note
            };

            // Publish the event to the message bus
            if (integrationEvent != null)
            {
                await publishEndpoint.Publish(integrationEvent, cancellationToken);

                logger.LogInformation("Published event: {EventName}", integrationEvent.GetType().Name);
            }
        }
    }
}
