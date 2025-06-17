using BuildingBlocks.Pagination;

namespace Appointment.API.Appointments.Queries
{
    public record GetAppointmentsResult(PaginatedResult<AppointmentSummaryDto> Appointments);
    public record GetAppointmentsQuery(int PageIndex, int PageSize) : IQuery<GetAppointmentsResult>;

    internal class GetAppointmentsQueryHandler : IQueryHandler<GetAppointmentsQuery, GetAppointmentsResult>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<GetAppointmentsResult> Handle(GetAppointmentsQuery query, CancellationToken cancellationToken)
        {
            PaginationHelper.VerifyPaginationRequest(query.PageIndex, query.PageSize);

            var appointments = await _appointmentRepository.GetAllAsync();

            if (appointments == null || !appointments.Any())
            {
                return new GetAppointmentsResult(new PaginatedResult<AppointmentSummaryDto>(query.PageIndex, query.PageSize, 0, new List<AppointmentSummaryDto>()));
            }

            var totalCount = appointments.Count();
            var paginatedAppointments = appointments
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(a => new AppointmentSummaryDto
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentType = a.AppointmentType.ToString(),
                    Note = a.Note,
                    IsSuspended = a.IsSuspended,
                    IsCancelled = a.IsCancelled
                })
                .ToList();

            var paginatedResult = new PaginatedResult<AppointmentSummaryDto>(query.PageIndex, query.PageSize, totalCount, paginatedAppointments);
            return new GetAppointmentsResult(paginatedResult);
        }
    }
}
