namespace Inventory.API.Endpoints
{
    public record GetMedicinesResponse(PaginatedResult<MedicineDTO> Medicines);

    public class GetMedicinesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/inventory/medicines", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetMedicinesQuery(request));
                var response = result.Adapt<GetMedicinesResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicines")
            .Produces<GetMedicinesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all medicines")
            .WithDescription("Get all medicines with pagination support.");
        }
    }
}
