using Inventory.Application.Medicines.Queries.GetMedicineRequireTest;

namespace Inventory.API.Endpoints
{
    public record GetVaccinesRequireTestResponse(PaginatedResult<VaccinesRequireTestDTO> Vaccines);

    public class GetVaccinesRequireTestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicines/vaccines-require-test", async (
                [AsParameters] PaginationRequest request,
                string? search,
                ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var result = await sender.Send(new ListVaccinesRequireTestQuery(request, search));
                var response = result.Adapt<GetVaccinesRequireTestResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetVaccinesRequireTest")
            .Produces<GetVaccinesRequireTestResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get vaccines that require testing before use")
            .WithDescription("Returns a paginated list of vaccines that require testing before use, with optional search filter.");
        }
    }
}