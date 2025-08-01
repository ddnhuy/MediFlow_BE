using BuildingBlocks.Strings;
using Inventory.Application.Medicines.Commands.ReturnMedicineBatch;

namespace Inventory.API.Endpoints
{
    public record CreateMedicineBatchReturnResponse(int Id);

    public class CreateMedicineBatchReturnEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/medicine-batch-returns", async (CreateMedicineBatchReturnCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                var response = result.Adapt<CreateMedicineBatchReturnResponse>();

                return Results.Created($"/inventory/medicine-batch-returns/{response.Id}", response);
            })
            .RequireAuthorization()
            .WithName("CreateMedicineBatchReturn")
            .Produces<CreateMedicineBatchReturnResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new medicine batch return")
            .WithDescription("Creates a new medicine batch return for expired batches");
        }
    }
}