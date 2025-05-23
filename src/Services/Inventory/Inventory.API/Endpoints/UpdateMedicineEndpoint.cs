namespace Inventory.API.Endpoints
{
    public record class UpdateMedicineResponse(bool IsSuccess);
    public class UpdateMedicineEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/medicines/{id}", async (int id, UpdateMedicineCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID mismatch between route and body");
                }

                var result = await sender.Send(command);

                var response = result.Adapt<UpdateMedicineResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateMedicine")
            .Produces<UpdateMedicineResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing medicine")
            .WithDescription("Updates an existing medicine record in the inventory system");
        }
    }
}
