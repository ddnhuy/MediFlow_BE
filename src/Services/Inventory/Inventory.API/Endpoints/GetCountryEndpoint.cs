namespace Inventory.API.Endpoints
{
    public record GetCountriesResponse(List<CountryDTO> Countries);
    public class GetCountryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/countries", async (ISender sender) =>
            {
                var result = await sender.Send(new GetCountriesQuery());
                var response = result.Adapt<GetCountriesResponse>();  
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetCountries")
            .Produces<GetCountriesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all countries")
            .WithDescription("Returns all active countries");
        }
    }
}
