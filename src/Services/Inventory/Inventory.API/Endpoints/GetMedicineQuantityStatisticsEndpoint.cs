namespace Inventory.API.Endpoints
{
    public record GetMedicineQuantityStatisticsResponse(PaginatedResult<MedicineQuantityStatisticsDto> Statistics);
    public class GetMedicineQuantityStatisticsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-quantity-statistics", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);

                var query = new GetMedicineQuantityStatisticsQuery(request);
                var result = await sender.Send(query);

                var respone = result.Adapt<GetMedicineQuantityStatisticsResponse>();

                return Results.Ok(result);
            }).RequireAuthorization()
            .WithName("GetMedicineQuantityStatistics")
            .Produces<GetMedicineQuantityStatisticsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get all medicine quantity statistics")
            .WithDescription("Get all medicine quantity statistics with pagination support");
        }
    }
}
