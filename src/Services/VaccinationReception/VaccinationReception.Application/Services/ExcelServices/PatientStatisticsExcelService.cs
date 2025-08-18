// src/Services/VaccinationReception/VaccinationReception.Application/Services/PatientStatisticsExcelService.cs
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using VaccinationReception.Application.DTOs.Reports;
using VaccinationReception.Application.Services.ExcelServices;

namespace VaccinationReception.Application.Services
{
    public class PatientStatisticsExcelService : IPatientStatisticsExcelService
    {
        public async Task<byte[]> GenerateExcelReportAsync(PatientStatisticsReportDTO reportData)
        {
            using var package = new ExcelPackage();

            // Summary overview sheet
            CreateSummarySheet(package, reportData);

            // Age group statistics sheet
            CreateAgeGroupSheet(package, reportData);

            // Location statistics sheet
            CreateLocationSheet(package, reportData);

            return await package.GetAsByteArrayAsync();
        }

        private void CreateSummarySheet(ExcelPackage package, PatientStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Tổng quan");

            // Header
            worksheet.Cells[1, 1, 1, 6].Merge = true;
            worksheet.Cells[1, 1].Value = "BÁO CÁO THỐNG KÊ BỆNH NHÂN";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1, 2, 6].Merge = true;
            worksheet.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[3, 1, 3, 6].Merge = true;
            worksheet.Cells[3, 1].Value = $"Ngày xuất: {reportData.GeneratedAt:dd/MM/yyyy HH:mm:ss} - Người xuất: {reportData.GeneratedBy}";
            worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Summary statistics
            int row = 5;
            worksheet.Cells[row, 1].Value = "THỐNG KÊ TỔNG QUAN";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.Size = 14;

            row += 2;
            worksheet.Cells[row, 1].Value = "Tổng số bệnh nhân:";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalPatients;
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            row += 2;
            worksheet.Cells[row, 1].Value = "THỐNG KÊ THEO NHÓM TUỔI";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.Size = 12;

            row++;
            // Age group summary table
            worksheet.Cells[row, 1].Value = "Nhóm tuổi";
            worksheet.Cells[row, 2].Value = "Độ tuổi";
            worksheet.Cells[row, 3].Value = "Số lượng";
            worksheet.Cells[row, 4].Value = "Tỷ lệ (%)";

            // Header styling
            for (int col = 1; col <= 4; col++)
            {
                worksheet.Cells[row, col].Style.Font.Bold = true;
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;
            foreach (var ageGroup in reportData.AgeGroupStatistics)
            {
                worksheet.Cells[row, 1].Value = ageGroup.AgeGroup;
                worksheet.Cells[row, 2].Value = ageGroup.AgeRange;
                worksheet.Cells[row, 3].Value = ageGroup.PatientCount;
                worksheet.Cells[row, 4].Value = ageGroup.Percentage;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "0.0";

                for (int col = 1; col <= 4; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;
            }

            row += 2;
            worksheet.Cells[row, 1].Value = "TOP ĐỊA PHƯƠNG";
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Font.Size = 12;

            row++;
            // Top 5 locations
            var topLocations = reportData.LocationStatistics.Take(5).ToList();
            foreach (var location in topLocations)
            {
                worksheet.Cells[row, 1].Value = $"{location.Stt}. {location.Province}";
                worksheet.Cells[row, 2].Value = $"{location.PatientCount} ({location.Percentage}%)";
                row++;
            }

            // Auto fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateAgeGroupSheet(ExcelPackage package, PatientStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Thống kê theo tuổi");

            // Header
            worksheet.Cells[1, 1, 1, 4].Merge = true;
            worksheet.Cells[1, 1].Value = "THỐNG KÊ BỆNH NHÂN THEO NHÓM TUỔI";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int headerRow = 3;
            string[] headers = { "Nhóm tuổi", "Độ tuổi", "Số lượng", "Tỷ lệ (%)" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
                worksheet.Cells[headerRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[headerRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[headerRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[headerRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Data rows
            int dataRow = headerRow + 1;
            foreach (var ageGroup in reportData.AgeGroupStatistics)
            {
                worksheet.Cells[dataRow, 1].Value = ageGroup.AgeGroup;
                worksheet.Cells[dataRow, 2].Value = ageGroup.AgeRange;
                worksheet.Cells[dataRow, 3].Value = ageGroup.PatientCount;
                worksheet.Cells[dataRow, 4].Value = ageGroup.Percentage;

                // Format percentage
                worksheet.Cells[dataRow, 4].Style.Numberformat.Format = "0.0";

                // Borders
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                dataRow++;
            }

            // Total row
            worksheet.Cells[dataRow, 1].Value = "TỔNG CỘNG";
            worksheet.Cells[dataRow, 2].Value = "";
            worksheet.Cells[dataRow, 3].Value = reportData.Summary.TotalPatients;
            worksheet.Cells[dataRow, 4].Value = 100.0;
            worksheet.Cells[dataRow, 4].Style.Numberformat.Format = "0.0";

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cells[dataRow, col].Style.Font.Bold = true;
                worksheet.Cells[dataRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[dataRow, col].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateLocationSheet(ExcelPackage package, PatientStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Thống kê theo địa phương");

            // Header
            worksheet.Cells[1, 1, 1, 4].Merge = true;
            worksheet.Cells[1, 1].Value = "THỐNG KÊ BỆNH NHÂN THEO ĐỊA PHƯƠNG";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int headerRow = 3;
            string[] headers = { "STT", "Tỉnh/Thành phố", "Số lượng", "Tỷ lệ (%)" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
                worksheet.Cells[headerRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[headerRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[headerRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[headerRow, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Data rows
            int dataRow = headerRow + 1;
            foreach (var location in reportData.LocationStatistics)
            {
                worksheet.Cells[dataRow, 1].Value = location.Stt;
                worksheet.Cells[dataRow, 2].Value = location.Province;
                worksheet.Cells[dataRow, 3].Value = location.PatientCount;
                worksheet.Cells[dataRow, 4].Value = location.Percentage;

                // Format percentage
                worksheet.Cells[dataRow, 4].Style.Numberformat.Format = "0.0";

                // Borders
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                dataRow++;
            }

            // Total row
            if (reportData.LocationStatistics.Any())
            {
                worksheet.Cells[dataRow, 1].Value = "";
                worksheet.Cells[dataRow, 2].Value = "TỔNG CỘNG";
                worksheet.Cells[dataRow, 3].Value = reportData.Summary.TotalPatients;
                worksheet.Cells[dataRow, 4].Value = 100.0;
                worksheet.Cells[dataRow, 4].Style.Numberformat.Format = "0.0";

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Font.Bold = true;
                    worksheet.Cells[dataRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[dataRow, col].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
    }
}