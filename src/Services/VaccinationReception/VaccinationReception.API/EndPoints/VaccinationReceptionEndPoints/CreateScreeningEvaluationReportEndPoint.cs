using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record CreateScreeningEvaluationResponse(int screeningEvaluationId);
    public class CreateScreeningEvaluationReportEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/screening-evaluations", async (CreateScreeningEvaluationReportCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                var response = new CreateScreeningEvaluationResponse(result.screeningEvaluationReportId);
                return Results.Created($"/screening-evaluations/{response.screeningEvaluationId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateScreeningEvaluation")
            .Produces<CreateScreeningEvaluationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Screening Evaluationt")
            .WithDescription("Create Screening Evaluation Record");
        }
    }
}