using BuildingBlocks.DomainEvents;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Strings.Enums;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
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
        IInventoryService inventoryService,
        IPublishEndpoint publishEndpoint) : INotificationHandler<ReceptionVaccinationCreatedEvent>
    {
        public async Task Handle(ReceptionVaccinationCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.AppointmentDate <= DateTime.UtcNow)
            {
                logger.LogWarning("Appointment date {AppointmentDate} is in the past for PatientId {PatientId}. Event will not be processed.",
                    notification.AppointmentDate, notification.PatientId);
                return;
            }

            var getPatient = patientGrpcClient.GetPatientAsync(notification.PatientId, cancellationToken);
            var getVaccine = inventoryService.GetMedicineInformationAsync([notification.VaccineId], cancellationToken);

            var patient = await getPatient;

            if (patient is null)
            {
                logger.LogWarning("No patient found for PatientId {PatientId}. Event will not be processed.", notification.PatientId);
                return;
            }

            if (string.IsNullOrEmpty(patient.Email))
            {
                logger.LogWarning("Patient with Id {PatientId} does not have an email. Event will not be processed.", notification.PatientId);
                return;
            }

            var vaccineList = await getVaccine;

            var vaccine = vaccineList.FirstOrDefault();

            if (vaccine is null || string.IsNullOrEmpty(vaccine.MedicineName))
            {
                logger.LogWarning("No vaccine found for VaccineId {VaccineId}. Event will not be processed.", notification.VaccineId);
                return;
            }

            var integrationEvent = new VaccinationIndicationWithAppointmentCreatedEvent
            {
                UserId = currentUserHelper.UserId,
                PatientId = notification.PatientId,
                AppointmentDate = notification.AppointmentDate,
                AppointmentType = AppointmentType.Vaccination,
                PatientCode = patient.Code,
                PatientFullName = patient.Name,
                PatientDOB = patient.DOB,
                PatientEmail = patient.Email,
                PatientPhoneNumber = patient.PhoneNumber,
                VaccineName = vaccine.MedicineName,
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
