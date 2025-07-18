using Appointment.API.Appointments.Queries;
using BuildingBlocks.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record GetUpcomingAppointmentsResponse(PaginatedResult<AppointmentSummaryDto> Appointments);

    public class GetUpcomingAppointmentsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/upcoming", [Authorize] async (DateTime? fromDate, DateTime? toDate, TimeOfDayFilter? timeOfDay,
                int? vaccineId, int pageIndex, int pageSize, ISender sender, ICurrentUserHelper helper) =>
            {
                var doctorId = helper.GetUserId();
                var result = await sender.Send(new GetUpcomingAppointmentsQuery(fromDate, toDate, doctorId, timeOfDay, vaccineId, pageIndex, pageSize));
                return Results.Ok(result.Adapt<GetUpcomingAppointmentsResponse>());
            })
            .WithName("GetUpcomingAppointments")
            .Produces<GetAppointmentByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Upcoming Appointments")
            .WithDescription("Get Upcoming Appointments");
        }
    }
}
