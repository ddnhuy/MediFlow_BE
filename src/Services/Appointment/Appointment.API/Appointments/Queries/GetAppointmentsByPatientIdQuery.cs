namespace Appointment.API.Appointments.Queries
{
    public record GetAppointmentsByPatientIdResult(IEnumerable<AppointmentSummaryDto> Appointments);
    public record GetAppointmentsByPatientIdQuery(int PatientId) : IQuery<GetAppointmentsByPatientIdResult>;

    internal class GetAppointmentsByPatientIdQueryHandler : IQueryHandler<GetAppointmentsByPatientIdQuery, GetAppointmentsByPatientIdResult>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentsByPatientIdQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<GetAppointmentsByPatientIdResult> Handle(GetAppointmentsByPatientIdQuery query, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(query.PatientId);

            if (appointments == null || !appointments.Any())
            {
                return new GetAppointmentsByPatientIdResult(new List<AppointmentSummaryDto>());
            }

            return new GetAppointmentsByPatientIdResult(appointments
                .Select(a => new AppointmentSummaryDto
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentType = a.AppointmentType.ToString(),
                    Note = a.Note,
                    IsSuspended = a.IsSuspended,
                    IsCancelled = a.IsCancelled
                }).ToList());
        }
    }
}
