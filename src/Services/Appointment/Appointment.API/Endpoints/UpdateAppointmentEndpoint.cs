using Appointment.API.Appointments.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record UpdateAppointmentResponse(bool IsSuccess, string Message);
    public record UpdateAppointmentRequest(int Id, int PatientId, int DepartmentId, DateTime AppointmentDate, AppointmentType AppointmentType, string PatientEmail, string? PatientPhoneNumber, string? Note, bool IsSuspended);

    public class UpdateAppointmentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/", [Authorize] async (UpdateAppointmentRequest request, ISender sender) =>
            {
                var result = await sender.Send(request.Adapt<UpdateAppointmentCommand>());

                return Results.Ok(result.Adapt<UpdateAppointmentResponse>());
            })
            .WithName("UpdateAppointment")
            .Produces<UpdateAppointmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Appointment")
            .WithDescription("Update Appointment");
        }
    }
}
