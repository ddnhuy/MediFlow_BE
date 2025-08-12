using Inventory.Application.Medicines.Queries.GetMedicineBatchesByMedicineId;

namespace Inventory.API.Endpoints
{
    public class GetMedicineBatchesByMedicineIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-quantity-statistics/medicines/{medicineId}/medicine-batches", async (int medicineId, [AsParameters] PaginationRequest paginationRequest, ISender mediator, string? batchNumber = null) =>
            {
                PaginationHelper.VerifyPaginationRequest(paginationRequest.PageIndex, paginationRequest.PageSize);

                var query = new GetMedicineBatchesByMedicineIdQuery(medicineId, paginationRequest, batchNumber);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }).RequireAuthorization()
            .Produces<GetMedicineBatchesByMedicineIdResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Get all medicine batches by medicine ID with pagination support");
        }
    }
}
