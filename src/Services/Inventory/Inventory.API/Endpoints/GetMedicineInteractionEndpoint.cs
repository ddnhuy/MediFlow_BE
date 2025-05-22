namespace Inventory.API.Endpoints
{
    public record GetMedicineInteractionsResponse(PaginatedResult<MedicineInteractionDTO> MedicineInteractions);

    public class GetMedicineInteractionsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/inventory/medicine-interactions", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetMedicineInteractionsQuery(request));
                var response = result.Adapt<GetMedicineInteractionsResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicineInteractions")
            .Produces<GetMedicineInteractionsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get all medicine interactions")
            .WithDescription("Get all medicine interactions with pagination support");
        }
    }
}
