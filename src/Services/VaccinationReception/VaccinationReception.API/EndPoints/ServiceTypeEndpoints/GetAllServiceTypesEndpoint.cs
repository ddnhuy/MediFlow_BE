using VaccinationReception.Application.Data;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.ServiceTypeEndpoints
{
    public record GetAllServiceTypesResponse(List<ServiceTypeDTO> ServiceTypes);

    public class GetAllServiceTypesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/service-types", async (ISender sender) =>
            {
                var query = new GetAllServiceTypesQuery();
                var result = await sender.Send(query);

                var response = new GetAllServiceTypesResponse(result);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllServiceTypes")
            .WithTags("ServiceType")
            .Produces<GetAllServiceTypesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all service types")
            .WithDescription("Get all active service types ordered by code");
        }
    }
}
