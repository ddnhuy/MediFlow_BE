using BuildingBlocks.CQRS;
using MediatR;
using VaccinationReception.Application.Abstraction.InventoryMessaging;

namespace VaccinationReception.Application.Vaccinations.Queries.GetNearestExpiryMedicineBatchWithMedicineId
{
    public class GetNearestExpiryMedicineBatchWithMedicineIdQueryHandler
        : IQueryHandler<GetNearestExpiryMedicineBatchWithMedicineIdQuery, GetNearestExpiryMedicineBatchWithMedicineIdResult>
    {
        private readonly IInventoryService _inventoryService;

        public GetNearestExpiryMedicineBatchWithMedicineIdQueryHandler(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<GetNearestExpiryMedicineBatchWithMedicineIdResult> Handle(
            GetNearestExpiryMedicineBatchWithMedicineIdQuery request,
            CancellationToken cancellationToken)
        {
            var response = await _inventoryService.GetNearestExpiryMedicineBatchAsync(request.MedicineId, cancellationToken);

            return new GetNearestExpiryMedicineBatchWithMedicineIdResult(
                response.MedicineBatches,
                response.RequestId,
                response.RequestedAt,
                response.IsSuccess,
                response.ErrorMessage
            );
        }
    }
}
