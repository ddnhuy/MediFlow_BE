namespace Inventory.API.Endpoints
{
    public record GetMedicineInteractionsByMedicineIdResponse(List<MedicineInteractionDTO> MedicineInteractions);

    public class GetMedicineInteractionByMedicineIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-interactions/medicines/{medicineId}", async (int medicineId, ISender sender) =>
            {
                var result = await sender.Send(new GetMedicineInteractionsByMedicineIdQuery(medicineId));
                
                var response = new GetMedicineInteractionsByMedicineIdResponse(result.MedicineInteractions);

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicineInteractionsByMedicineId")
            .Produces<GetMedicineInteractionsByMedicineIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get medicine interactions by medicine ID")
            .WithDescription("Returns all interactions where the specified medicine is involved as either medicine 1 or medicine 2");
        }
    }
}
