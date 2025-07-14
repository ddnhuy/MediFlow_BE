using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetAllServicesWithDetailsResponse(List<ServiceDetailDTO> Services);

    public class GetAllServicesWithDetailsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/services/details", async (ISender sender) =>
            {
                var query = new GetAllServicesWithDetailsQuery();
                var result = await sender.Send(query);

                var response = new GetAllServicesWithDetailsResponse(result);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllServicesWithDetails")
            .WithTags("Service")
            .Produces<GetAllServicesWithDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all services with full details")
            .WithDescription("Get all active services with complete information including service groups and disease groups");
        }
    }
}