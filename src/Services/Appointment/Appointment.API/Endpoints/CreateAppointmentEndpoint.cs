using Appointment.API.Appointments.Commands;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Appointment.API.Endpoints
{
    public record CreateAppointmentResponse(bool IsSuccess, string Message);
    public record CreateAppointmentRequest(int PatientId, DateTime AppointmentDate, AppointmentType AppointmentType, string PatientCode, string PatientFullName, DateTime PatientDOB, string PatientEmail, string? PatientPhoneNumber, string? VaccineName, string? Note, int DoctorId, int VaccineId, string? Dose);

    public class CreateAppointmentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/", [Authorize] async (CreateAppointmentRequest request, ISender sender, HttpContext httpContext) =>
            {
                var command = new CreateAppointmentCommand(
                    int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    request.PatientId,
                    request.AppointmentDate,
                    request.AppointmentType,
                    request.PatientCode,
                    request.PatientFullName,
                    request.PatientDOB,
                    request.PatientEmail,
                    request.PatientPhoneNumber,
                    request.VaccineName,
                    request.Note,
                    request.DoctorId,
                    request.VaccineId,
                    request.Dose);

                var result = await sender.Send(command);

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
