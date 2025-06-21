using BuildingBlocks.CQRS;
using MediatR;

namespace VaccinationReception.Application.Vaccinations.Queries.GetNearestExpiryMedicineBatchWithMedicineId
{
    public record GetNearestExpiryMedicineBatchWithMedicineIdQuery(int MedicineId)
        : IQuery<GetNearestExpiryMedicineBatchWithMedicineIdResult>;

    public record GetNearestExpiryMedicineBatchWithMedicineIdResult(
        int MedicineId,
        string? MedicineName,
        int MedicineBatchId,
        string? MedicineBatchNumber,
        DateOnly? ExpiryDate,
        string? RequestId,
        DateTime RequestedAt,
        bool IsSuccess,
        string? ErrorMessage
    );
}
