
using Inventory.Application.Medicines.Queries.GetMedicineInteractionById;

namespace Inventory.API.Endpoints
{
    public class GetMedicineInteractionByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-interactions/{id}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetMedicineInteractionByIdQuery(id));
                
                return Results.Ok(result.MedicineInteraction);
            }).RequireAuthorization()
            .WithName("GetMedicineInteractionById")
            .Produces<MedicineInteractionDTO>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get medicine interaction by ID")
            .WithDescription("Returns the details of a specific medicine interaction by its ID.");
        }
    }
}
