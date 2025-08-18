using VaccinationReception.Application.ReceptionVaccinationContracts.Handlers;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints
{
    public record UpdateContractStatusRequest(
            ContractStatus Status,
            string? Reason = null
        );

    public class UpdateContractStatusEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/contracts/{contractId}/status",
                async (int contractId, [FromBody] UpdateContractStatusRequest request, ISender sender) =>
                {
                    var command = new UpdateContractStatusCommand(
                        contractId,
                        request.Status,
                        request.Reason
                    );

                    var result = await sender.Send(command);

                    return result
                        ? Results.Ok("Contract status updated successfully")
                        : Results.Problem("Failed to update contract status");
                })
            .RequireAuthorization()
            .WithName("UpdateContractStatus")
            .WithTags("Contract")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update contract status")
            .WithDescription("Update contract status including cancellation. When cancelling a contract, automatically updates all related entities such as payment contracts, receptions, and vaccination records.");
        }
    }
}
