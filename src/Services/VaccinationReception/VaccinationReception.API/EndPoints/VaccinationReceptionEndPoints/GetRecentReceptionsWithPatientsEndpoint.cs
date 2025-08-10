using BuildingBlocks.Pagination;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record GetRecentReceptionsWithPatientsResponse(PaginatedResult<RecentReceptionWithPatientDTO> Receptions);

    public class GetRecentReceptionsWithPatientsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/receptions/recent-with-patients", async (
                [AsParameters] PaginationRequest paginationRequest,
                string? searchTerm,
                ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(paginationRequest.PageIndex, paginationRequest.PageSize);

                var query = new GetRecentReceptionsWithPatientsQuery(paginationRequest, searchTerm);
                var result = await sender.Send(query);

                var response = new GetRecentReceptionsWithPatientsResponse(result.Receptions);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetRecentReceptionsWithPatients")
            .Produces<GetRecentReceptionsWithPatientsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get recent receptions with patient information")
            .WithDescription("Retrieves reception IDs and patient information for receptions updated within the last 2 hours with pagination and search support. Search by patient code, name, phone number, or identity card.");
        }
    }
}