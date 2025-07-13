
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Vaccinations.Queries.GetNearestExpiryMedicineBatchWithMedicineId;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetNearestExpiryMedicineBatchWithMedicineIdResponse(List<GetNearestExpiryMedicineBatchItem> MedicineBatches,
        string? RequestId,
        DateTime RequestedAt,
        bool IsSuccess,
        string? ErrorMessage);

    public class GetNearestExpiryMedicineBatchWithMedicineIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/nearest-expiry-medicine-batch/{medicineId}", async (int medicineId, ISender sender) =>
            {
                var query = new GetNearestExpiryMedicineBatchWithMedicineIdQuery(medicineId);
                var result = await sender.Send(query);
                var response = result.Adapt<GetNearestExpiryMedicineBatchWithMedicineIdResponse>();

                return Results.Ok(response);

            }).RequireAuthorization()
            .RequireAuthorization()
            .WithName("GetNearestExpiryMedicineBatchWithMedicineId")
            .Produces<GetNearestExpiryMedicineBatchWithMedicineIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Nearest Expiry MedicineBatch")
            .WithDescription("Get Nearest Expiry MedicineBatch");
        }
    }
}
