using BuildingBlocks.Pagination;
using Carter;
using Mapster;
using MediatR;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Patients.Queries.ListPatients;

namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record ListPatientsResponse(PaginatedResult<PatientSummaryDTO> Patients);
    public class ListPatientsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new ListPatientsQuery(request));
                var response = result.Adapt<ListPatientsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("ListPatients")
            .Produces<ListPatientsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get all patients")
            .WithDescription("Get all patients with pagination support");
        }
    }
}