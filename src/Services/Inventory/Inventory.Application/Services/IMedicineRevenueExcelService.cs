namespace Inventory.Application.Services
{
    public interface IMedicineRevenueExcelService
    {
        Task<byte[]> GenerateExcelReportAsync(MedicineRevenueReportDTO reportData);
    }
}
