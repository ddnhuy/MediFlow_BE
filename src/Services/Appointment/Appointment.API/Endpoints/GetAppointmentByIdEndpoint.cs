using Appointment.API.Appointments.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Appointment.API.Endpoints
{
    public record GetAppointmentByIdResponse(AppointmentDetailDto Appointment);

    public class GetAppointmentByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/{appointmentId}", [Authorize] async (int appointmentId, ISender sender, HttpContext httpContext) =>
            {
                var result = await sender.Send(new GetAppointmentByIdQuery(appointmentId, httpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value));

                return Results.Ok(result.Adapt<GetAppointmentByIdResponse>());
            })
            .WithName("GetAppointmentById")
            .Produces<GetAppointmentByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Appointment")
            .WithDescription("Get Appointment By Id");
        }
    }
}
