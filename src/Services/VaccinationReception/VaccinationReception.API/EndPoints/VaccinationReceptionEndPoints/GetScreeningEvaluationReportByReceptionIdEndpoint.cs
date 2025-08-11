using BuildingBlocks.Strings;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record GetScreeningEvaluationReportByReceptionIdResponse(ScreeningEvaluationReportDTO? Report);

    public class GetScreeningEvaluationReportByReceptionIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/receptions/{receptionId:int}/screening-evaluation-report", async (
                int receptionId,
                ISender sender) =>
            {
                if (receptionId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
                }

                var query = new GetScreeningEvaluationReportByReceptionIdQuery(receptionId);
                var result = await sender.Send(query);

                var response = new GetScreeningEvaluationReportByReceptionIdResponse(result.Report);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetScreeningEvaluationReportByReceptionId")
            .Produces<GetScreeningEvaluationReportByReceptionIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get screening evaluation report by reception ID")
            .WithDescription("Retrieves the screening evaluation report for a specific reception. Returns null if no report exists for the reception.");
        }
    }
}
