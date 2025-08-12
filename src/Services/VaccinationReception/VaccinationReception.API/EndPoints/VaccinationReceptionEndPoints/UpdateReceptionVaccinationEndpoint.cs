using BuildingBlocks.Strings;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record UpdateReceptionVaccinationRequest(int Id, int Quantity, bool IsReadyToUse, DateTime? ScheduledDate, DateTime AppointmentDate, string? Note);

    public record UpdateReceptionVaccinationResponse(bool IsSuccess);

    public class UpdateReceptionVaccinationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/receptions/{receptionId}/reception-vaccinations",
                async (int receptionId, [FromBody] UpdateReceptionVaccinationRequest request, ISender sender) =>
                {
                    var command = new UpdateReceptionVaccinationCommand(
                       Id: request.Id,
                       ReceptionId: receptionId,
                       Quantity: request.Quantity,
                       IsReadyToUse: request.IsReadyToUse,
                       ScheduledDate: request.ScheduledDate,
                       AppointmentDate: request.AppointmentDate,
                       Note: request.Note
                    );

                    var result = await sender.Send(command);

                    var response = result.Adapt<UpdateReceptionVaccinationResponse>();
                    return Results.Ok(response);
                })
            .RequireAuthorization()
            .WithName("UpdateReceptionVaccination")
            .Produces<UpdateReceptionVaccinationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update a reception vaccination")
            .WithDescription("Updates an existing reception vaccination record by reception ID");
        }
    }
}