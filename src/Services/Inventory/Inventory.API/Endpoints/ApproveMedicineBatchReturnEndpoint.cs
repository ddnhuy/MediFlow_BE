using Inventory.Application.Medicines.Commands.ApproveMedicineBatchReturn;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Endpoints
{
    public record ApproveMedicineBatchReturnRequest(string Token);
    public class ApproveMedicineBatchReturnEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/medicine-batch-returns/{id}/approve", async (int id, [FromBody] ApproveMedicineBatchReturnRequest request, ISender sender) =>
            {
                var command = new ApproveMedicineBatchReturnCommand(id, request.Token);
                var result = await sender.Send(command);

                return Results.Ok(new { message = "Medicine batch return has been approved successfully" });
            })
            .WithName("ApproveMedicineBatchReturn")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Approve a medicine batch return")
            .WithDescription("Approves a pending medicine batch return");
        }
    }
}
