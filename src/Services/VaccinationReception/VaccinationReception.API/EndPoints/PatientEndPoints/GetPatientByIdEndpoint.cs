using BuildingBlocks.Strings;

namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record GetPatientByIdResponse(PatientDetailDTO Patient);
    public class GetPatientByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    throw new BadRequestException(ExceptionKey.INVALID_PATIENT_ID);
                }

                var query = new GetPatientQuery(id);
                var result = await sender.Send(query);

                if (result == null)
                {
                    throw new NotFoundException(ExceptionKey.NOT_FOUND_PATIENT_WITH_ID);
                }

                return Results.Ok(new GetPatientByIdResponse(result.Patient));
            })
            .RequireAuthorization()
            .WithName("GetPatientById")
            .Produces<GetPatientByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get patient by Id")
            .WithDescription("Get patient details by Id");
        }
    }
}