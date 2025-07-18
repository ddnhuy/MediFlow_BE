using BuildingBlocks.Messaging.Contracts.Email;
using BuildingBlocks.Messaging.Enums.BuildingBlocks.Messaging.Enums;
using MassTransit;
using Quartz;

namespace Appointment.API.Jobs
{
    public class DailyAppointmentNotificationJob : IJob
    {
        private readonly ILogger<DailyAppointmentNotificationJob> _logger;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrentUserHelper _currentUserHelper;

        public DailyAppointmentNotificationJob(ILogger<DailyAppointmentNotificationJob> logger, IAppointmentRepository appointmentRepository, IPublishEndpoint publishEndpoint, ICurrentUserHelper currentUserHelper)
        {
            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _publishEndpoint = publishEndpoint;
            _currentUserHelper = currentUserHelper;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Executing Daily Appointment Notification Job at {Time}",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")));

            var today = DateTime.UtcNow.Date;
            var currentUserId = _currentUserHelper.GetUserId();

            var (appointments, totalCount) = await _appointmentRepository.GetUpcomingAppointmentsAsync(fromDate: today, pageIndex: 1, pageSize: int.MaxValue); 

            if (appointments.Any())
            {
                _logger.LogInformation("Found {Count} upcoming appointments for next day.", appointments.Count());

                var tasks = appointments
                    .Where(appointment => appointment.AppointmentDate.DayOfYear == today.DayOfYear + 1 && !appointment.IsSuspended && !appointment.IsCancelled)
                    .Select(appointment =>
                    _publishEndpoint.Publish(new SendEmailMessage
                    {
                        To = appointment.PatientEmail!,
                        SubjectCode = EmailSubjectCode.AppointmentReminder,
                        TemplateData = new Dictionary<string, string>
                        {
                            ["AppointmentDate"] = appointment.AppointmentDate.AddHours(7).ToString("HH:mm dd/MM/yyyy"),
                            ["AppointmentType"] = appointment.AppointmentType.ToString(),
                            ["PatientCode"] = appointment.PatientCode,
                            ["PatientFullName"] = appointment.PatientFullName,
                            ["PatientDOB"] = appointment.PatientDOB.ToString("dd/MM/yyyy"),
                            ["VaccineName"] = appointment.VaccineName ?? "N/A",
                            ["Note"] = appointment.Note ?? "N/A"
                        }
                    }, context.CancellationToken)
                );

                await Task.WhenAll(tasks);
            }
            else
            {
                _logger.LogInformation("No upcoming appointments found for today.");
            }
        }
    }
}
