using VaccinationReception.Application.HospitalFees.Commands;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public record RefundPaymentRequest(
            PaymentMethod Method,
            string? Note,
            List<int> RefundedReceptionVaccinationIds,
            List<int> RefundedServiceRequestDetailIds);

    public record RefundPaymentResponse(int RefundPaymentId);

    public class RefundPaymentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/receptions/{receptionId:int}/payments/{originalPaymentId:int}/refund",
                async (int receptionId, int originalPaymentId, RefundPaymentRequest request, ISender sender) =>
                {
                    var command = new RefundPaymentCommand(
                        receptionId,
                        originalPaymentId,
                        request.Method,
                        request.Note,
                        request.RefundedReceptionVaccinationIds,
                        request.RefundedServiceRequestDetailIds
                    );

                    var result = await sender.Send(command);
                    var response = result.Adapt<RefundPaymentResponse>();

                    return Results.Created($"/payments/{response.RefundPaymentId}", response);
                })
            .RequireAuthorization()
            .WithName("RefundPayment")
            .Produces<RefundPaymentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Refund a payment for specific items")
            .WithDescription("Creates a refund payment record for selected items from an original payment, marking them as unpaid.");
        }
    }
}
