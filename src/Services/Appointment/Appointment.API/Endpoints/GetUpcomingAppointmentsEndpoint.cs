using Appointment.API.Appointments.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record GetUpcomingAppointmentsResponse(IEnumerable<AppointmentSummaryDto> Appointments);

    public class GetUpcomingAppointmentsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/upcoming", [Authorize] async (ISender sender) =>
            {
                var result = await sender.Send(new GetUpcomingAppointmentsQuery(DateTime.UtcNow));

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
