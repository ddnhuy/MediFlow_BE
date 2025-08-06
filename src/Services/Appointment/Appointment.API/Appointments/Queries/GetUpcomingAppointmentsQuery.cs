using BuildingBlocks.Pagination;

namespace Appointment.API.Appointments.Queries
{
    public record GetUpcomingAppointmentsResult(PaginatedResult<AppointmentSummaryDto> Appointments);
    public record GetUpcomingAppointmentsQuery(
        DateTime? FromDate,
        DateTime? ToDate,
        int DoctorId,
        TimeOfDayFilter? TimeOfDay,
        int? VaccineId,
        int PageIndex = 1,
        int PageSize = 10
    ) : IQuery<GetUpcomingAppointmentsResult>;

    internal class GetUpcomingAppointmentsQueryHandler : IQueryHandler<GetUpcomingAppointmentsQuery, GetUpcomingAppointmentsResult>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetUpcomingAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<GetUpcomingAppointmentsResult> Handle(GetUpcomingAppointmentsQuery query, CancellationToken cancellationToken)
        {
            var (appointments, totalCount) = await _appointmentRepository.GetUpcomingAppointmentsAsync(query.FromDate, query.ToDate, query.DoctorId, query.TimeOfDay, query.VaccineId, query.PageIndex, query.PageSize);

            if (appointments == null || !appointments.Any())
            {
                return new GetUpcomingAppointmentsResult(new PaginatedResult<AppointmentSummaryDto>(query.PageIndex, query.PageSize, 0, []));
            }

            var result = new PaginatedResult<AppointmentSummaryDto>(
                query.PageIndex,
                query.PageSize,
                appointments.Count(),
                appointments.Select(a => new AppointmentSummaryDto
                {
                    Id = a.Id,
                    PatientCode = a.PatientCode,
                    PatientName = a.PatientFullName ?? string.Empty,
                    PatientAge = DateTime.Today.Year - a.PatientDOB.Year,
                    VaccineName = a.VaccineName ?? string.Empty,
                    Dose = a.Dose ?? string.Empty,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentType = a.AppointmentType.ToString(),
                    Note = a.Note,
                    IsSuspended = a.IsSuspended,
                    IsCancelled = a.IsCancelled
                })
                .OrderByDescending(a => a.AppointmentDate)
            );

            return new GetUpcomingAppointmentsResult(result);
        }
    }
}
