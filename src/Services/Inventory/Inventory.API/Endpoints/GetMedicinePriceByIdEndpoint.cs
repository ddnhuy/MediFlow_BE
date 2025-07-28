using Inventory.Application.Medicines.Queries.GetMedicinePriceById;

namespace Inventory.API.Endpoints
{
    public record GetMedicinePriceByIdResponse(MedicinePriceDTO MedicinePrice);

    public class GetMedicinePriceByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-prices/{id}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetMedicinePriceByIdQuery(id));
                var response = result.Adapt<GetMedicinePriceByIdResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicinePriceById")
            .Produces<GetMedicinePriceByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get medicine price by ID")
            .WithDescription("Get a specific medicine price by its ID");
        }
    }
}