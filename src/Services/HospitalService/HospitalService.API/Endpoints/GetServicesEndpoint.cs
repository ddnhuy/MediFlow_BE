using BuildingBlocks.Pagination;
using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetServicesResponse(PaginatedResult<ServiceDTO> Services);

    public class GetServicesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/services", async ([AsParameters] PaginationRequest request, string? searchTerm, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var query = new GetServicesQuery(
                    PaginationRequest: new PaginationRequest(request.PageIndex, request.PageSize),
                    SearchTerm: searchTerm
                );

                var result = await sender.Send(query);

                var response = result.Adapt<GetServicesResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetServices")
            .WithTags("Service")
            .Produces<GetServicesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get services with pagination")
            .WithDescription("Get services with pagination and search support. Search can be applied on service code and service name.");
        }
    }
}
