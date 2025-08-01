using BuildingBlocks.Strings.Enums;
using Inventory.Application.Medicines.Queries.GetMedicineBatchReturns;

namespace Inventory.API.Endpoints
{
    public class GetMedicineBatchReturnsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-batch-returns", async (
                ISender sender,
                [AsParameters] PaginationRequest pagination,
                string? searchReturnCode = null,
                MedicineBatchReturnStatus? status = null) =>
            {
                PaginationHelper.VerifyPaginationRequest(pagination.PageIndex, pagination.PageSize);

                var query = new GetMedicineBatchReturnsQuery(
                    Pagination: pagination,
                    SearchReturnCode: searchReturnCode,
                    Status: status.HasValue ? (MedicineBatchReturnStatus)status.Value : null
                );
                var result = await sender.Send(query);               
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetMedicineBatchReturns")
            .Produces<GetMedicineBatchReturnsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get all medicine batch return requests with pagination, search and filtering")
            .WithDescription("Retrieves medicine batch return requests with pagination. Supports searching by return code and filtering by status (0=Pending, 1=Approved, 2=Rejected)");
        }
    }
}