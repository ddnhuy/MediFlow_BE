using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.ReceptionVaccinationContracts.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record GetAllContractsResponse(PaginatedResult<ContractResponse> Contracts);

    public class GetAllContractsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/contracts", async (
                [AsParameters] PaginationRequest paginationRequest,
                string? searchTerm,
                ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(paginationRequest.PageIndex, paginationRequest.PageSize);

                var query = new GetAllContractsQuery(paginationRequest, searchTerm);
                var result = await sender.Send(query);

                var response = new GetAllContractsResponse(result);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllContracts")
            .WithTags("Contract")
            .Produces<GetAllContractsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all contracts")
            .WithDescription("Retrieves all contracts with pagination and search support. Search by contract code, name, company name, unit name, or description.");
        }
    }
}
