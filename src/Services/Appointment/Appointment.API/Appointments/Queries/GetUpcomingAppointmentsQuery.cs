namespace Appointment.API.Appointments.Queries
{
    public record GetUpcomingAppointmentsResult(IEnumerable<AppointmentSummaryDto> Appointments);
    public record GetUpcomingAppointmentsQuery(DateTime FromDate) : IQuery<GetUpcomingAppointmentsResult>;

    internal class GetUpcomingAppointmentsQueryHandler : IQueryHandler<GetUpcomingAppointmentsQuery, GetUpcomingAppointmentsResult>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetUpcomingAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<GetUpcomingAppointmentsResult> Handle(GetUpcomingAppointmentsQuery query, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetUpcomingAppointmentsAsync(query.FromDate);

            if (appointments == null || !appointments.Any())
            {
                return new GetUpcomingAppointmentsResult(new List<AppointmentSummaryDto>());
            }

            return new GetUpcomingAppointmentsResult(appointments
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
