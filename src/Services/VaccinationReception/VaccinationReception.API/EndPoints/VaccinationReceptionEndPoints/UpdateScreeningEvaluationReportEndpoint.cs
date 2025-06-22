using BuildingBlocks.Strings;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record UpdateScreeningEvaluationReportResponse(bool IsSuccess);
    public class UpdateScreeningEvaluationReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/screeninge-valuation/{id}", async (int id, [FromBody] UpdateScreeningEvaluationReportCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
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