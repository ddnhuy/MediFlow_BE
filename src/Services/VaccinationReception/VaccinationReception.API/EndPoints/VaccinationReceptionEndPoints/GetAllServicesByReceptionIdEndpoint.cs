using BuildingBlocks.Strings;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record GetAllServicesByReceptionIdResponse(IEnumerable<ServiceRequestDetailDTO> Services);

    public class GetAllServicesByReceptionIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/receptions/{receptionId}/services", async (int receptionId, ISender sender) =>
            {
                if (receptionId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID);
                }

                var query = new GetAllServicesByReceptionIdQuery(receptionId);
                var result = await sender.Send(query);

                if (result == null || !result.Any())
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_SERVICE_REQUEST);
                }

                var response = new GetAllServicesByReceptionIdResponse(result);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetAllServicesByReceptionId")
            .Produces<GetAllServicesByReceptionIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all services by reception ID")
            .WithDescription("Retrieves all service request details associated with a specific reception");
        }
    }
}