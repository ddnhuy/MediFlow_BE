using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record UpdateScreeningEvaluationReportResponse(bool IsSuccess);
    public class UpdateScreeningEvaluationReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/screeningevaluation/{id}", async (int id, [FromBody] UpdateScreeningEvaluationReportCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID trong đường dẫn không khớp với ID trong nội dung yêu cầu");
                }

                var result = await sender.Send(command);
                var response = result.Adapt<UpdateScreeningEvaluationReportResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateScreeningEvaluationReport")
            .Produces<UpdateScreeningEvaluationReportResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing screening evaluation report")
            .WithDescription("Updates an existing screening evaluation report record");
        }
    }
}