using VaccinationReception.Application.HospitalFees.Queries;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.HospitalFeeEndpoints
{
    public class GetPaymentStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/payment-status", async (
                int? paymentId,
                int? paymentContractId,
                ISender sender) =>
            {
                var query = new GetPaymentStatusQuery(paymentId, paymentContractId);
                var result = await sender.Send(query);

                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetPaymentStatus")
            .WithTags("Payment Status")
            .Produces<PaymentStatus>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get payment status by ID")
            .WithDescription("Retrieves the status of a payment or payment contract. Provide either paymentId or paymentContractId (at least one is required).");
        }
    }
}
