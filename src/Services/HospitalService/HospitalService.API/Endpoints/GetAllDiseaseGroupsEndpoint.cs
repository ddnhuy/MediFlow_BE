using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetAllDiseaseGroupsResponse(List<DiseaseGroupDTO> DiseaseGroups);

    public class GetAllDiseaseGroupsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/disease-groups/all", async (string? searchTerm, ISender sender) =>
            {
                var query = new GetAllDiseaseGroupsQuery(searchTerm);
                var result = await sender.Send(query);

                var response = result.Adapt<GetAllDiseaseGroupsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllDiseaseGroups")
            .Produces<GetAllDiseaseGroupsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all disease groups")
            .WithDescription("Get all disease groups with search support");
        }
    }
}
