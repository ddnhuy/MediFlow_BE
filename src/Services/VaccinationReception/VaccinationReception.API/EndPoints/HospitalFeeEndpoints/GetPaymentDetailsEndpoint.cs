using BuildingBlocks.Strings;
using VaccinationReception.Application.HospitalFees.Queries;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public record GetPaymentDetailsResponse(
            PaymentResponse Payment,
            List<PaymentDetailResponse> PaymentDetails
        );
    public record PaymentDetailResponse(
        int Id,
        int PaymentId,
        int? ReceptionVaccinationId,
        int? ServiceRequestDetailId,
        decimal Amount,
        bool IsReversed,
        DateTime CreatedAt,
        DateTime LastUpdatedAt,
        string? ServiceCode,
        string? ServiceName
    );
    public class GetPaymentDetailsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/payments/{paymentId:int}/details", async (int paymentId, ISender sender) =>
            {
                if(paymentId <= 0)
                {
                    throw new BadRequestException(ExceptionKey.PAYMENT_ID_INVALID);
                }
                var query = new GetPaymentDetailsQuery(paymentId);
                var result = await sender.Send(query);

                var paymentResponse = new PaymentResponse(
                    result.Payment.Id,
                    result.Payment.ReceptionId,
                    result.Payment.TotalAmount,
                    result.Payment.Method,
                    result.Payment.Note,
                    result.Payment.ATMTransactionCode,
                    result.Payment.PaymentType.ToString(),
                    result.Payment.InvoiceNumber,
                    result.Payment.OfficialInvoiceNumber,
                    result.Payment.Status?.ToString(),
                    result.Payment.OriginalPaymentId,
                    result.Payment.CreatedAt,
                    result.Payment.LastUpdatedAt
                );

                var paymentDetailsResponse = result.PaymentDetails.Select(pd => new PaymentDetailResponse(
                    pd.Id,
                    pd.PaymentId,
                    pd.ReceptionVaccinationId,
                    pd.ServiceRequestDetailId,
                    pd.Amount,
                    pd.IsReversed,
                    pd.CreatedAt,
                    pd.LastUpdatedAt,
                    pd.ServiceCode,
                    pd.ServiceName
                )).ToList();

                var response = new GetPaymentDetailsResponse(paymentResponse, paymentDetailsResponse);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetPaymentDetails")
            .Produces<GetPaymentDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get payment details by payment ID")
            .WithDescription("Retrieves detailed information about a specific payment including all its payment details.");
        }
    }
}
