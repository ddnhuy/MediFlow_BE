namespace Inventory.API.Endpoints
{
    public record GetMedicinePricesByMedicineIdResponse(MedicinePriceDTO MedicinePrices);

    public class GetMedicinePricesByMedicineIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-prices/{medicineId}", async (int medicineId, ISender sender) =>
            {
                if (medicineId <= 0)
                {
                    return Results.BadRequest("Id không hợp lệ");
                }

                var result = await sender.Send(new GetMedicinePricesByMedicineIdQuery(medicineId));
                var response = result.Adapt<GetMedicinePricesByMedicineIdResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicinePricesByMedicineId")
            .Produces<GetMedicinePricesByMedicineIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get medicine prices by medicine ID")
            .WithDescription("Get all prices for a specific medicine by its ID");
        }
    }
}
