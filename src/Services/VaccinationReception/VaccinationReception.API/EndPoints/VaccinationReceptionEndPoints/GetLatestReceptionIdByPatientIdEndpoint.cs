using BuildingBlocks.Strings;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class GetLatestReceptionIdByPatientIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/{patientId}/latest-reception-id", async (
                int patientId,
                ISender sender) =>
            {
                if (patientId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_PATIENT_ID);
                }

                var query = new GetLatestReceptionIdByPatientIdQuery(patientId);
                var result = await sender.Send(query);

                if (result is null || result == 0)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_PATIENT_ID);
                }

                return Results.Ok(new { ReceptionId = result });
            })
            .RequireAuthorization()
            .WithName("GetLatestReceptionIdByPatientId")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get latest reception id by patient ID")
            .WithDescription("Retrieves the latest reception id for a specific patient");
        }
    }
}
