using VaccinationReception.Application.ReceptionVaccinationContracts.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public class GetPatientsByContractIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/contracts/{contractId}/patients", async (
                int contractId,
                [AsParameters] PaginationRequest pagination,
                string ? searchTerm,
                ISender sender) =>
            {
                if (contractId <= 0)
                {
                    return Results.BadRequest("Contract ID must be greater than 0");
                }

                var query = new GetPatientsByContractIdQuery(contractId, pagination, searchTerm ?? string.Empty);

                var result = await sender.Send(query);

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetPatientsByContractId")
            .WithTags("Contract")
            .Produces<PaginatedResult<PatientSummaryDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get patients by contract ID")
            .WithDescription("Retrieves all patients associated with a specific contract, with optional search and pagination.");
        }
    }
}