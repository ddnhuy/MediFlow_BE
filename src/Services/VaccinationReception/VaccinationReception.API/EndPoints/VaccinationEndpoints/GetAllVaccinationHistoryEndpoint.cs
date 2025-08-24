using VaccinationReception.Application.Vaccinations.Queries.GetAllVaccinationHistory;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetAllVaccinationHistoryResponse(PaginatedResult<AllVaccinationHistoryItem> VaccinationHistory);

    public class GetAllVaccinationHistoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/history", async (
                [AsParameters] PaginationRequest paginationRequest,
                DateTime? fromDate,
                DateTime? toDate,
                string? searchTerm,
                ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(paginationRequest.PageIndex, paginationRequest.PageSize);

                var query = new GetAllVaccinationHistoryQuery(
                    paginationRequest,
                    fromDate,
                    toDate,
                    searchTerm
                );

                var result = await sender.Send(query);
                var response = new GetAllVaccinationHistoryResponse(result);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllVaccinationHistory")
            .WithTags("Vaccination")
            .Produces<GetAllVaccinationHistoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all vaccination history")
            .WithDescription("Retrieves all vaccination history with pagination, date filtering (defaults to last 30 days), and search support. " +
                           "Results are ordered by vaccination date descending. Search by patient name, patient code, medicine name, or medicine type. " +
                           "Date filtering is based on actual vaccination date from the Vaccination table.");
        }
    }
}