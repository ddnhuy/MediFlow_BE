using VaccinationReception.Application.DTOs.Reports;

namespace VaccinationReception.Application.Services.ExcelServices
{
    public interface IHospitalRevenueExcelService
    {
        Task<byte[]> GenerateExcelReportAsync(HospitalRevenueReportDTO reportData);
    }
}
