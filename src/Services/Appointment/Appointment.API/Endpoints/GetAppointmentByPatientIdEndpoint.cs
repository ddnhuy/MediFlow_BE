using Appointment.API.Appointments.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record GetAppointmentsByPatientIdResponse(IEnumerable<AppointmentSummaryDto> Appointments);

    public class GetAppointmentsByPatientIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/{patientId}/appointments", [Authorize] async (int patientId, ISender sender, HttpContext httpContext) =>
            {
                var result = await sender.Send(new GetAppointmentsByPatientIdQuery(patientId));

                return Results.Ok(result.Adapt<GetAppointmentsByPatientIdResponse>());
            })
            .WithName("GetAppointmentsByPatientId")
            .Produces<GetAppointmentsByPatientIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Appointments")
            .WithDescription("Get Appointments By PatientId");
        }
    }
}
