using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetAllServiceGroupsResponse(List<ServiceGroupDTO> ServiceGroups);

    public class GetAllServiceGroupsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/service-groups/all", async (string? searchTerm, ISender sender) =>
            {
                var query = new GetAllServiceGroupsQuery(searchTerm);
                var result = await sender.Send(query);

                var response = result.Adapt<GetAllServiceGroupsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllServiceGroups")
            .Produces<GetAllServiceGroupsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all service groups")
            .WithDescription("Get all service groups with search support");
        }
    }
}