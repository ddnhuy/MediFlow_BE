using BuildingBlocks.Strings;

namespace Inventory.API.Endpoints
{
    public record CreateMedicineResponse(int Id);
    public class CreateMedicineEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/medicines", async (CreateMedicineCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                var response = result.Adapt<CreateMedicineResponse>();

                return Results.Created($"/inventory/medicines/{response.Id}", response);
            })
            .RequireAuthorization()
            .WithName("CreateMedicine")
            .Produces<CreateMedicineInteractionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new medicine ")
            .WithDescription("Creates a new medicine"); ;          
        }
    }
}
