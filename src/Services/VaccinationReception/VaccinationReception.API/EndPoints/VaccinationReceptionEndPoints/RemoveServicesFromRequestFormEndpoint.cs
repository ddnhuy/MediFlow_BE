using BuildingBlocks.Strings;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record RemoveServicesFromRequestFormResponse(int RequestFormId);

    public class RemoveServicesFromRequestFormEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/request-forms/{receptionId}/services", async (
                int receptionId,
                [FromBody] List<int> serviceIds,
                ISender sender) =>
            {
                if (receptionId <= 0 || serviceIds is not { Count: > 0 } || serviceIds.Any(id => id < 0))
                {
                    throw new BadRequestException(ExceptionKey.INVALID_REQUEST);
                }

                var command = new RemoveServicesFromRequestFormCommand(receptionId, serviceIds);
                var result = await sender.Send(command);
                var response = result.Adapt<RemoveServicesFromRequestFormResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("RemoveServicesFromRequestForm")
            .Produces<RemoveServicesFromRequestFormResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Cancel services from request form")
            .WithDescription("Cancels one or more services from a request form by setting IsCancelled to true. If all services are cancelled, the request form will also be marked as cancelled.");
        }
    }
}