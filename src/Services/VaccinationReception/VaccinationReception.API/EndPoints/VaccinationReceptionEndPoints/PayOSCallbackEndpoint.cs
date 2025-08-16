using System.Text.Json;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using Microsoft.AspNetCore.Authorization;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class PayOSCallbackEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/payment-callback/payos", async ([FromBody] JsonElement payload, ISender sender) =>
            {
                var command = new ProcessPayOSCallbackCommand(payload);
                var result = await sender.Send(command);

                if (result.Success)
                {
                    return Results.Ok(new { message = result.Message });
                }
                else
                {
                    return Results.BadRequest(new { message = result.Message });
                }
            })
            .AllowAnonymous()
            .WithName("PayOSCallback")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Handle PayOS payment callback")
            .WithDescription("Receives and processes PayOS payment callback notifications.");
        }
    }
}
