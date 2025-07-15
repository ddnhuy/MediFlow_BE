using BuildingBlocks.CQRS;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using MediatR;

namespace VaccinationReception.Application.Vaccinations.Queries.GetNearestExpiryMedicineBatchWithMedicineId
{
    public record GetNearestExpiryMedicineBatchWithMedicineIdQuery(int MedicineId)
        : IQuery<GetNearestExpiryMedicineBatchWithMedicineIdResult>;

    public record GetNearestExpiryMedicineBatchWithMedicineIdResult(
        List<GetNearestExpiryMedicineBatchItem> MedicineBatches,
        string? RequestId,
        DateTime RequestedAt,
        bool IsSuccess,
        string? ErrorMessage
    );
}
