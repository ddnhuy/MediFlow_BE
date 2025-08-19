namespace Inventory.Application.Services
{
    public interface IInventoryStatisticsExcelService
    {
        Task<byte[]> GenerateExcelReportAsync(InventoryStatisticsReportDTO reportData);
    }
}
