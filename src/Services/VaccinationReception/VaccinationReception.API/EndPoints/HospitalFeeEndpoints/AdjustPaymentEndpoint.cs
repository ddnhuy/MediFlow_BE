using VaccinationReception.Application.HospitalFees.Commands;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public record AdjustPaymentRequest(
           PaymentMethod Method,
           string? Note,
           List<int> CancelledReceptionVaccinationIds,
           List<int> CancelledServiceRequestDetailIds,
           List<int> NewReceptionVaccinationIds,
           List<int> NewServiceRequestDetailIds);

    public record AdjustPaymentResponse(int AdjustmentPaymentId);

    public class AdjustPaymentEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/receptions/{receptionId:int}/payments/{originalPaymentId:int}/adjust",
                async (int receptionId, int originalPaymentId, AdjustPaymentRequest request, ISender sender) =>
                {
                    var command = new AdjustPaymentCommand(
                        receptionId,
                        originalPaymentId,
                        request.Method,
                        request.Note,
                        request.CancelledReceptionVaccinationIds,
                        request.CancelledServiceRequestDetailIds,
                        request.NewReceptionVaccinationIds,
                        request.NewServiceRequestDetailIds
                    );

                    var result = await sender.Send(command);

                    var response = result.Adapt<AdjustPaymentResponse>();

                    return Results.Created($"/payments/{response.AdjustmentPaymentId}", response);
                })
            .RequireAuthorization()
            .WithName("AdjustPayment")
            .Produces<AdjustPaymentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Adjust an existing payment")
            .WithDescription("Creates an adjustment payment to cancel old items and add new ones, calculating the net difference.");
        }
    }
}
