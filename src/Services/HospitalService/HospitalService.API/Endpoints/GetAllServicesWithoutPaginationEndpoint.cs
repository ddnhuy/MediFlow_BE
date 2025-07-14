using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using Mapster;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetAllServicesWithoutPaginationResponse(List<ServiceDTO> Services);

    public class GetAllServicesWithoutPaginationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/services/all", async (string? searchTerm, ISender sender) =>
            {
                var query = new GetAllServicesWithoutPaginationQuery(searchTerm);
                var result = await sender.Send(query);

                var response = result.Adapt<GetAllServicesWithoutPaginationResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllServicesWithoutPagination")
            .Produces<GetAllServicesWithoutPaginationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all services without pagination")
            .WithDescription("Get all services with search support. If no search term provided, returns all services. Search by ID, service code, or service name.");
        }
    }
}