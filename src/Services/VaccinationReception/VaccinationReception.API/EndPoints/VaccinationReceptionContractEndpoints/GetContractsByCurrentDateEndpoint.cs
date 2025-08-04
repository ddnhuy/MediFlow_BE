using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.ReceptionVaccinationContracts.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record GetContractsByCurrentDateResponse(List<ContractResponse> Contracts);

    public class GetContractsByCurrentDateEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/contracts/current-date", async (ISender sender) =>
            {
                var query = new GetContractsByCurrentDateQuery();
                var result = await sender.Send(query);

                var response = new GetContractsByCurrentDateResponse(result);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetContractsByCurrentDate")
            .WithTags("Contract")
            .Produces<GetContractsByCurrentDateResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get contracts by current date")
            .WithDescription("Retrieves all contracts that have a contract date or expected date matching today's date.");
        }
    }
}
