using BuildingBlocks.Strings;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class GetUnpaidServicesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/receptions/{receptionId}/unpaid-services", async (
                int receptionId,
                ISender sender) =>
            {
                if (receptionId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
                }

                var query = new GetUnpaidServicesQuery(receptionId);
                var result = await sender.Send(query);

                if (result == null ||
                    (!result.Services.Any() && !result.Vaccinations.Any()))
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_UNPAID_SERVICES_WITH_RECEPTION_ID);
                }

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetUnpaidServices")
            .Produces<UnpaidServicesResponseDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get unpaid services and vaccinations")
            .WithDescription("Retrieves a list of unpaid services and vaccinations for a specific reception");
        }
    }
}