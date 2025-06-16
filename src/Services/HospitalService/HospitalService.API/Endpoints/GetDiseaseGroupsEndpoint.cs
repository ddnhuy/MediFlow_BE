using BuildingBlocks.Pagination;
using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetDiseaseGroupsResponse(PaginatedResult<DiseaseGroupDTO> DiseaseGroups);

    public class GetDiseaseGroupsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/disease-groups", async ([AsParameters] PaginationRequest request, string? searchTerm, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var query = new GetDiseaseGroupsQuery(
                    PaginationRequest: new PaginationRequest(request.PageIndex, request.PageSize),
                    SearchTerm: searchTerm
                );

                var result = await sender.Send(query);

                var response = result.Adapt<GetDiseaseGroupsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetDiseaseGroups")
            .Produces<GetDiseaseGroupsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get disease groups")
            .WithDescription("Get disease groups with pagination and search support");
        }
    }
}
