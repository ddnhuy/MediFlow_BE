using VaccinationReception.Application.HospitalFees.Commands;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public record CreatePaymentRequest(
            PaymentMethod Method,
            string? Note,
            [FromQuery] List<int> ReceptionVaccinationIds,
            [FromQuery] List<int> ServiceRequestDetailIds);

    public record CreatePaymentResponse(int PaymentId);

    public class CreatePaymentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/receptions/{receptionId:int}/payments", async (int receptionId, CreatePaymentRequest request, ISender sender) =>
            {
                var command = new CreatePaymentCommand(
                    receptionId,
                    request.Method,
                    request.Note,
                    request.ReceptionVaccinationIds,
                    request.ServiceRequestDetailIds
                );

                var result = await sender.Send(command);

                var response = result.Adapt<CreatePaymentResponse>();

                return Results.Created($"/payments/{response.PaymentId}", response);
            })
            .RequireAuthorization()
            .WithName("CreatePayment")
            .Produces<CreatePaymentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create a new payment for a reception")
            .WithDescription("Creates a new payment for selected vaccination items and services within a reception.");
        }
    }
}
