using Inventory.Application.Medicines.Queries.GetVaccineTypes.Inventory.Application.VaccineTypes.Queries;

namespace Inventory.API.Endpoints
{
    public record GetVaccineTypesResponse(List<VaccineTypeDTO> VaccineTypes);

    public class GetVaccineTypesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccine-types", async (ISender sender) =>
            {
                var result = await sender.Send(new GetVaccineTypesQuery());
                var response = new GetVaccineTypesResponse(result.VaccineTypes);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetVaccineTypes")
            .Produces<GetVaccineTypesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all vaccine types")
            .WithDescription("Returns all vaccine types.");
        }
    }
}