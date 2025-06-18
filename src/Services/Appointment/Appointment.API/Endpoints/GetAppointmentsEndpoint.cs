using Appointment.API.Appointments.Queries;
using BuildingBlocks.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace Appointment.API.Endpoints
{
    public record GetAppointmentsResponse(PaginatedResult<AppointmentSummaryDto> Appointments);

    public class GetAppointmentsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/appointments", [Authorize] async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetAppointmentsQuery(request.PageIndex, request.PageSize));

                return Results.Ok(result.Adapt<GetAppointmentsResponse>());
            })
            .WithName("GetAppointments")
            .Produces<GetAppointmentByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Appointments")
            .WithDescription("Get Appointments");
        }
    }
}
