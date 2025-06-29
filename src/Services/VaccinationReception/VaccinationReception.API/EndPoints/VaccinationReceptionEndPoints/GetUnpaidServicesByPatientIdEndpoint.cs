using BuildingBlocks.Strings;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class GetUnpaidServicesByPatientIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/{patientId}/unpaid-services", async (
                int patientId,
                ISender sender) =>
            {
                if (patientId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_PATIENT_ID);
                }

                var query = new GetUnpaidServicesByPatientIdQuery(patientId);
                var result = await sender.Send(query);

                if (result == null ||
                    (!result.Services.Any() && !result.Vaccinations.Any()))
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_UNPAID_SERVICES_WITH_PATIENT_ID);
                }

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetUnpaidServicesByPatientId")
            .Produces<UnpaidServicesResponseDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get unpaid services and vaccinations by patient ID")
            .WithDescription("Retrieves a list of unpaid services and vaccinations for a specific patient across all their receptions");
        }
    }
}
