using Carter;
using HospitalService.Application.DTOs;
using HospitalService.Application.Services.HospitalServices.Queries;
using MediatR;

namespace HospitalService.API.Endpoints
{
    public record GetAllExaminationServicesResponse(List<ServiceDTO> Services);

    public class GetAllExaminationServicesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/services/examination", async (string? searchTerm, ISender sender) =>
            {
                var query = new GetAllExaminationServicesQuery(searchTerm);
                var result = await sender.Send(query);
                var response = new GetAllExaminationServicesResponse(result.Services);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllExaminationServices")
            .Produces<GetAllExaminationServicesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all examination services")
            .WithDescription("Get all examination services with optional search support.");
        }
    }
}