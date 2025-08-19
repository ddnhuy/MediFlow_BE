using VaccinationReception.Application.DTOs.Reports;

namespace VaccinationReception.Application.Services.ExcelServices
{
    public interface IPatientStatisticsExcelService
    {
        Task<byte[]> GenerateExcelReportAsync(PatientStatisticsReportDTO reportData);
    }
}
