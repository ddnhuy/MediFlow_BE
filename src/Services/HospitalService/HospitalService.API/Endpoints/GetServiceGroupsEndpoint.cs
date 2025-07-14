using BuildingBlocks.Pagination;
using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetServiceGroupsResponse(PaginatedResult<ServiceGroupDTO> ServiceGroups);

    public class GetServiceGroupsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/service-groups", async ([AsParameters] PaginationRequest request, string? searchTerm, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var query = new GetServiceGroupsQuery(
                    PaginationRequest: new PaginationRequest(request.PageIndex, request.PageSize),
                    SearchTerm: searchTerm
                );

                var result = await sender.Send(query);

                var response = result.Adapt<GetServiceGroupsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetServiceGroups")
            .Produces<GetServiceGroupsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get service groups")
            .WithDescription("Get service groups with pagination and search support");
        }
    }
}