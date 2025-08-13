using BuildingBlocks.Strings;
using Grpc.Core;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services
{
    public class InventoryService(
        ILogger<InventoryService> logger,
        ApplicationDbContext dbContext) : InventoryProtoService.InventoryProtoServiceBase
    {
        public override async Task<GetTrafficByVaccineResponse> GetTrafficByVaccine(GetTrafficByVaccineRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetTrafficByVaccine called with request: {Request}", request);

            var vaccineTraffic = await dbContext.InventoryHistories
                .AsNoTracking()
                .Where(x => x.TransactionType == InventoryTransactionType.EXPORT)
                .Include(x => x.Medicine)
                .GroupBy(x => new { x.MedicineId, x.Medicine!.MedicineName })
                .Select(g => new VaccineTrafficData
                {
                    VaccineId = g.Key.MedicineId,
                    VaccineName = g.Key.MedicineName,
                    TotalUsed = g.LongCount()
                })
                .OrderByDescending(x => x.TotalUsed)
                .ToListAsync(context.CancellationToken);

            logger.LogInformation("Vaccine traffic data retrieved: {Count} records", vaccineTraffic.Count);

            return new GetTrafficByVaccineResponse
            {
                Data = { vaccineTraffic }
            };
        }
    }
}
