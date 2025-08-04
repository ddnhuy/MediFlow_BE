using VaccinationReception.Application.ReceptionVaccinationContracts.Handlers;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record ReceiveContractPatientRequest(int ContractId, int PatientId);
    public class ReceptionContractPatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/contracts/receive-patient", async ([FromBody] ReceiveContractPatientRequest request, ISender sender) =>
            {
                var command = new ReceptionContractPatientCommand(request.ContractId, request.PatientId);
                var result = await sender.Send(command);

                return Results.Created($"/receptions/{result}", result);
            })
            .RequireAuthorization()
            .WithName("ReceiveContractPatient")
            .WithTags("Contract")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Receive contract patient")
            .WithDescription("Receives a patient for a contract by creating a reception and adding corresponding vaccinations.");
        }
    }
}