namespace Inventory.API.Endpoints
{
    public record GetMedicineByIdResponse(MedicineDTO Medicine);
    public class GetMedicineByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicines/{id}", async (int id, ISender sender) =>
            {
                var query = new GetMedicineByIdQuery(id);
                var result = await sender.Send(query);

                if (result == null)
                {
                    return Results.NotFound(InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_ID(id));
                }

                var medicineDTO = result.Medicine.Adapt<MedicineDTO>();
                return Results.Ok(new GetMedicineByIdResponse(medicineDTO));
            })
            .RequireAuthorization()
            .WithName("GetMedicineById")
            .Produces<GetMedicineByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get medicine by Id")
            .WithDescription("Get medicine by Id");
        }
    }
}
