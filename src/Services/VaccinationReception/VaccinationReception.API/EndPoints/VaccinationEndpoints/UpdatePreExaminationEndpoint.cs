using VaccinationReception.Application.Vaccinations.Commands.UpdatePreExaminationResult;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record UpdatePreExaminationResponse(bool IsSucess);
    public class UpdatePreExaminationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/pre-examination/{receptionVaccinationId}/result", async (int receptionVaccinationId, [FromBody] UpdatePreExaminationCommand command, ISender sender) =>
            {
                if (receptionVaccinationId != command.ReceptionVaccinationId)
                {
                    return Results.BadRequest("Reception Vaccination ID mismatch.");
                }

                var result = await sender.Send(command);
                var response = result.Adapt<UpdatePreExaminationResponse>();
                return Results.Ok(response);
            }).RequireAuthorization()
              .Produces<UpdatePreExaminationResponse>(StatusCodes.Status200OK)
              .WithName("UpdatePreExamination")
              .WithSummary("Update Pre-Examination Result for Vaccination Reception")
              .WithDescription("Updates the pre-examination result for a specific vaccination reception by ID.");
        }
    }
}
