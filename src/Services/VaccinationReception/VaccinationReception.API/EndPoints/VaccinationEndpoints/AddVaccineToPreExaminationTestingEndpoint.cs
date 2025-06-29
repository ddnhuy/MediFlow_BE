
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record AddVaccineToPreExaminationTestingResponse(bool IsSuccess);
    public class AddVaccineToPreExaminationTestingEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/pre-examination/{receptionVaccinationId}", async (int receptionVaccinationId, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new AddVaccineToPreExaminationTestingCommand(receptionVaccinationId);
                var result = await sender.Send(command, cancellationToken);
                var response = result.Adapt<AddVaccineToPreExaminationTestingResponse>();
                return Results.Ok(response);
            }).RequireAuthorization()
              .WithName("AddVaccineToPreExaminationTesting")
              .Produces<AddVaccineToPreExaminationTestingResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Add vaccine to pre-examination testing")
              .WithDescription("Adds a vaccine to the pre-examination testing for a specific reception vaccination.");
        }
    }
}
