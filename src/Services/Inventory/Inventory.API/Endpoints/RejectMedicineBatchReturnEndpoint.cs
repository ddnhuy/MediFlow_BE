using Inventory.Application.Medicines.Commands.RejectMedicineBatchReturn;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Endpoints
{
    public record RejectMedicineBatchReturnRequest(string Token);
    public class RejectMedicineBatchReturnEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/medicine-batch-returns/{id}/reject", async (int id, [FromBody] RejectMedicineBatchReturnRequest request, ISender sender) =>
            {
                var command = new RejectMedicineBatchReturnCommand(id, request.Token);
                var result = await sender.Send(command);

                return Results.Ok(new { message = "Medicine batch return hass been rejected successfully" });
            })
            .WithName("RejectMedicineBatchReturn")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Reject a medicine batch return")
            .WithDescription("Rejects a pending medicine batch return");
        }
    }
}
