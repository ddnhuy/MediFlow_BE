using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record PatientReceptionResponse(int patientId, int receptionId);
    public class PatientReceptionEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/patient-reception", async (CreatePatientReceptionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    throw new InternalServerException("Tạo bệnh nhân thất bại");
                }

                var response = new PatientReceptionResponse(result.patientId, result.receptionId);
                return Results.Created($"/patients/{response.patientId}", response);
            })
            .RequireAuthorization()
            .WithName("PatientReception")
            .Produces<PatientReceptionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new patient")
            .WithDescription("Creates a new patient record");
        }
    }
}