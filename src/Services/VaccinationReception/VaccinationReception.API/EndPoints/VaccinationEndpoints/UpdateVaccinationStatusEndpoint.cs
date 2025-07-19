
using VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationStatus;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record UpdateVaccinationStatusCommandResponse(bool IsSuccess);
    public class UpdateVaccinationStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/vaccination/{vaccinationId}/status", async (int vaccinationId, UpdateVaccinationStatusCommand command, ISender sender) =>
            {
                // Ensure the command has the correct ReceptionVaccinationId
                if (command.VaccinationId != vaccinationId)
                {
                    return Results.BadRequest("The ReceptionVaccinationId in the command does not match the ID in the URL.");
                }

                var result = await sender.Send(command);
                var response = result.Adapt<UpdateVaccinationStatusCommandResponse>();
                return Results.Ok(response);
            }).RequireAuthorization()
              .WithName("UpdateVaccinationStatus")
              .Produces<UpdateVaccinationStatusCommandResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Vaccination Status")
              .WithDescription("Update the status of a vaccination reception. The request must include the receptionVaccinationId in the URL and the new status in the request body.");
        }
    }
}
