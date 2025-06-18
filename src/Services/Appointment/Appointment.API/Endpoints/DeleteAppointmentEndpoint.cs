using Appointment.API.Appointments.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record DeleteAppointmentResponse(bool IsSuccess, string Message);

    public class DeleteAppointmentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/appointments/{appointmentId}", [Authorize] async (int appointmentId, ISender sender) =>
            {
                var result = await sender.Send(new DeleteAppointmentCommand(appointmentId));

                if (!result.IsSuccess)
                {
                    throw new BadRequestException(result.Message);
                }
                return Results.Ok(result.Adapt<DeleteAppointmentResponse>());
            })
            .WithName("DeleteAppointment")
            .Produces<DeleteAppointmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Appointment")
            .WithDescription("Delete Appointment");
        }
    }
}
