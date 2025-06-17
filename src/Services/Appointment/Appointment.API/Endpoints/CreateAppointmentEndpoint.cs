using Appointment.API.Appointments.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record CreateAppointmentResponse(bool IsSuccess, string Message);
    public record CreateAppointmentRequest(int PatientId, int DepartmentId, DateTime AppointmentDate, AppointmentType AppointmentType, string PatientEmail, string? PatientPhoneNumber, string? Note);

    public class CreateAppointmentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/appointments", [Authorize] async (CreateAppointmentRequest request, ISender sender) =>
            {
                var result = await sender.Send(request.Adapt<CreateAppointmentCommand>());

                if (!result.IsSuccess)
                {
                    throw new BadRequestException(result.Message);
                }
                return Results.Created();
            })
            .WithName("CreateAppointment")
            .Produces<CreateAppointmentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Appointment")
            .WithDescription("Create Appointment");
        }
    }
}
