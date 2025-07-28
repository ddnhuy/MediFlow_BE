using BuildingBlocks.Strings;
using VaccinationReception.Application.DTOs.VaccinationDTOs;
using VaccinationReception.Application.Vaccinations.Queries.GetVaccinationDetailById;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public class GetVaccinationDetailByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/{id}/detail", async (int id, ISender sender) =>
            {
                var query = new GetVaccinationDetailByIdQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetVaccinationDetailById")
            .Produces<VaccinationDetailDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Vaccination Detail By Id")
            .WithDescription("Retrieves detailed vaccination information for a given vaccination record.");
        }
    }
}