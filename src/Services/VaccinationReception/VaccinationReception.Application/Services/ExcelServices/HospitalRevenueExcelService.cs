// src/Services/VaccinationReception/VaccinationReception.Application/Services/HospitalRevenueExcelService.cs
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using VaccinationReception.Application.DTOs.Reports;
using VaccinationReception.Application.Services.ExcelServices;

namespace VaccinationReception.Application.Services
{
    public class HospitalRevenueExcelService : IHospitalRevenueExcelService
    {
        public async Task<byte[]> GenerateExcelReportAsync(HospitalRevenueReportDTO reportData)
        {
            using var package = new ExcelPackage();

            // Main summary sheet
            CreateSummarySheet(package, reportData);

            // Daily revenue details sheet
            CreateDailyRevenueSheet(package, reportData);

            return await package.GetAsByteArrayAsync();
        }

        private void CreateSummarySheet(ExcelPackage package, HospitalRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Tổng quan doanh thu");

            // Header
            worksheet.Cells[1, 1, 1, 6].Merge = true;
            worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU BỆNH VIỆN";
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

            // Revenue summary table
            worksheet.Cells[row, 1].Value = "Loại doanh thu";
            worksheet.Cells[row, 2].Value = "Số lượng";
            worksheet.Cells[row, 3].Value = "Doanh thu (VNĐ)";

            // Header styling
            for (int col = 1; col <= 3; col++)
            {
                worksheet.Cells[row, col].Style.Font.Bold = true;
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            // Exam fee revenue
            worksheet.Cells[row, 1].Value = "Tiền khám";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalExamCount;
            worksheet.Cells[row, 3].Value = reportData.Summary.TotalExamFeeRevenue;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0 ₫";

            for (int col = 1; col <= 3; col++)
            {
                worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            // Test fee revenue
            worksheet.Cells[row, 1].Value = "Tiền xét nghiệm";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalTestCount;
            worksheet.Cells[row, 3].Value = reportData.Summary.TotalTestFeeRevenue;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0 ₫";

            for (int col = 1; col <= 3; col++)
            {
                worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            // Vaccination revenue
            worksheet.Cells[row, 1].Value = "Số công tiêm";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalInjectionCount;
            worksheet.Cells[row, 3].Value = reportData.Summary.TotalInjectionRevenue;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0 ₫";

            for (int col = 1; col <= 3; col++)
            {
                worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            // Total revenue
            worksheet.Cells[row, 1].Value = "TỔNG CỘNG";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalExamCount + reportData.Summary.TotalTestCount + reportData.Summary.TotalExamCount;
            worksheet.Cells[row, 3].Value = reportData.Summary.TotalRevenue;
            worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0 ₫";

            for (int col = 1; col <= 3; col++)
            {
                worksheet.Cells[row, col].Style.Font.Bold = true;
                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row += 3;

            // Additional statistics
            worksheet.Cells[row, 1].Value = "Doanh thu trung bình/ngày:";
            worksheet.Cells[row, 2].Value = reportData.Summary.AverageDailyRevenue;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            // Auto fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateDailyRevenueSheet(ExcelPackage package, HospitalRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Chi tiết theo ngày");

            // Header
            worksheet.Cells[1, 1, 1, 8].Merge = true;
            worksheet.Cells[1, 1].Value = "CHI TIẾT DOANH THU THEO NGÀY";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int headerRow = 3;
            string[] headers = {
                "Ngày",
                "SL Công khám", "Tiền khám (VNĐ)",
                "SL XN", "Tiền Xét nghiệm (VNĐ)",
                "SL Công tiêm", "Tiền tiêm (VNĐ)",
                "Tổng doanh thu (VNĐ)"
            };

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
            foreach (var dailyRevenue in reportData.DailyRevenues.OrderBy(d => d.Date))
            {
                worksheet.Cells[dataRow, 1].Value = dailyRevenue.Date.ToString("dd/MM/yyyy");
                worksheet.Cells[dataRow, 2].Value = dailyRevenue.ExamCount;
                worksheet.Cells[dataRow, 3].Value = dailyRevenue.ExamFeeRevenue;
                worksheet.Cells[dataRow, 4].Value = dailyRevenue.TestCount;
                worksheet.Cells[dataRow, 5].Value = dailyRevenue.TestFeeRevenue;
                worksheet.Cells[dataRow, 6].Value = dailyRevenue.InjectionCount;
                worksheet.Cells[dataRow, 7].Value = dailyRevenue.InjectionRevenue;
                worksheet.Cells[dataRow, 8].Value = dailyRevenue.TotalRevenue;

                // Format currency columns
                worksheet.Cells[dataRow, 3].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 5].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 7].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 8].Style.Numberformat.Format = "#,##0 ₫";

                // Highlight total revenue column
                worksheet.Cells[dataRow, 8].Style.Font.Bold = true;

                // Borders
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                dataRow++;
            }

            // Total row
            if (reportData.DailyRevenues.Any())
            {
                worksheet.Cells[dataRow, 1].Value = "TỔNG CỘNG";
                worksheet.Cells[dataRow, 2].Value = reportData.Summary.TotalExamCount;
                worksheet.Cells[dataRow, 3].Value = reportData.Summary.TotalExamFeeRevenue;
                worksheet.Cells[dataRow, 4].Value = reportData.Summary.TotalTestCount;
                worksheet.Cells[dataRow, 5].Value = reportData.Summary.TotalTestFeeRevenue;
                worksheet.Cells[dataRow, 6].Value = reportData.Summary.TotalInjectionCount;
                worksheet.Cells[dataRow, 7].Value = reportData.Summary.TotalInjectionRevenue;
                worksheet.Cells[dataRow, 8].Value = reportData.Summary.TotalRevenue;

                // Format currency columns
                worksheet.Cells[dataRow, 3].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 5].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 7].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 8].Style.Numberformat.Format = "#,##0 ₫";

                // Style total row
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