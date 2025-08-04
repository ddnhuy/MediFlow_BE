using VaccinationReception.Application.ReceptionVaccinationContracts.Handlers;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record UpdatePaymentContractStatusRequest(
        PaymentStatus PaymentStatus
    );
    public class UpdatePaymentContractStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/contracts/{contractId}/payment-contracts/{paymentContractId}/status",
                async (int contractId, int paymentContractId, [FromBody] UpdatePaymentContractStatusRequest request, ISender sender) =>
                {
                    var command = new UpdatePaymentContractStatusCommand(
                        contractId,
                        paymentContractId,
                        request.PaymentStatus
                    );

                    var result = await sender.Send(command);

                    return result
                        ? Results.Ok("Status updated successfully")
                        : Results.Problem("Failed to update status");
                })
            .RequireAuthorization()
            .WithName("UpdatePaymentContractStatus")
            .WithTags("Contract")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update payment contract status")
            .WithDescription("Update status for payment contract and contract. If status is Completed or Cancelled, update both payment contract and contract status.");
        }
    }
}