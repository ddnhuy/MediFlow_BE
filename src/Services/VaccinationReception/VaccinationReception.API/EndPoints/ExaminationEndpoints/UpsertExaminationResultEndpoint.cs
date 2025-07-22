using VaccinationReception.Application.Examinations.Handlers;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class UpsertExaminationResultEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("examination/results", async ([FromBody] UpsertExaminationResultCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            }).RequireAuthorization()
            .Produces<UpsertExaminationResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription("Upsert Examination Result");
        }
    }
}
