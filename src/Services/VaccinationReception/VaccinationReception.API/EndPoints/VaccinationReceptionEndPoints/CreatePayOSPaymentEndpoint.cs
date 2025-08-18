using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record CreatePayOSPaymentRequest(
           int ReceptionId,
           PaymentMethod Method,
           string? Note,
           List<int> ReceptionVaccinationIds,
           List<int> ServiceRequestDetailIds);

    public record CreatePayOSPaymentResponse(int PaymentId, string InvoiceNumber, string CheckoutUrl, string QrCode);

    public class CreatePayOSPaymentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/receptions/{patientId:int}/payos-payment", async (int patientId, CreatePayOSPaymentRequest request, ISender sender) =>
            {
                var command = new CreatePayOSPaymentCommand(
                    patientId,
                    request.ReceptionId,
                    request.Method,
                    request.Note,
                    request.ReceptionVaccinationIds,
                    request.ServiceRequestDetailIds
                );

                var result = await sender.Send(command);

                var response = new CreatePayOSPaymentResponse(result.PaymentId, result.InvoiceNumber, result.CheckoutUrl, result.QrCode);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("CreatePayOSPayment")
            .Produces<CreatePayOSPaymentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create a new PayOS payment for a reception")
            .WithDescription("Creates a new PayOS payment for selected vaccination items and services within a reception and returns checkout URL.");
        }
    }
}
