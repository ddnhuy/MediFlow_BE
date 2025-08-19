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

            CreateSummarySheet(package, reportData);
            CreateDailyRevenueSheet(package, reportData);

            return await package.GetAsByteArrayAsync();
        }

        #region Helpers
        private void ApplyHeaderStyle(ExcelRange cell, Color bgColor)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(bgColor);
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private void ApplyCellBorder(ExcelRange cell) =>
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

        private void ApplyCurrencyFormat(ExcelRange cell) =>
            cell.Style.Numberformat.Format = "#,##0 ₫";

        private void CreateHeaderRow(ExcelWorksheet ws, int row, string[] headers, Color bgColor)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[row, i + 1];
                cell.Value = headers[i];
                ApplyHeaderStyle(cell, bgColor);
            }
        }

        private void CreateDataRow(ExcelWorksheet ws, int row, object[] values, int[] currencyCols = null, int? boldCol = null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var cell = ws.Cells[row, i + 1];
                cell.Value = values[i];
                ApplyCellBorder(cell);

                if (currencyCols != null && currencyCols.Contains(i + 1))
                    ApplyCurrencyFormat(cell);

                if (boldCol.HasValue && i + 1 == boldCol.Value)
                    cell.Style.Font.Bold = true;
            }
        }

        private void CreateTotalRow(ExcelWorksheet ws, int row, object[] values, int colCount, int[] currencyCols = null)
        {
            for (int i = 0; i < colCount; i++)
            {
                var cell = ws.Cells[row, i + 1];
                cell.Value = values[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                ApplyCellBorder(cell);

                if (currencyCols != null && currencyCols.Contains(i + 1))
                    ApplyCurrencyFormat(cell);
            }
        }
        #endregion

        private void CreateSummarySheet(ExcelPackage package, HospitalRevenueReportDTO reportData)
        {
            var ws = package.Workbook.Worksheets.Add("Tổng quan doanh thu");

            // Report title
            ws.Cells[1, 1, 1, 6].Merge = true;
            ws.Cells[1, 1].Value = "BÁO CÁO DOANH THU BỆNH VIỆN";
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.Font.Size = 16;
            ws.Cells[1, 1].Style.Font.Bold = true;

            ws.Cells[2, 1, 2, 6].Merge = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";

            ws.Cells[3, 1, 3, 6].Merge = true;
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[3, 1].Value = $"Ngày xuất: {reportData.GeneratedAt:dd/MM/yyyy HH:mm:ss} - Người xuất: {reportData.GeneratedBy}";

            // Section title
            int row = 5;
            ws.Cells[row, 1].Value = "THỐNG KÊ TỔNG QUAN";
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.Font.Size = 14;
            row += 2;

            // Header
            CreateHeaderRow(ws, row, new[] { "Loại doanh thu", "Số lượng", "Doanh thu (VNĐ)" }, Color.LightGray);
            row++;

            // Data rows
            CreateDataRow(ws, row++, new object[] { "Tiền khám", reportData.Summary.TotalExamCount, reportData.Summary.TotalExamFeeRevenue }, new[] { 3 });
            CreateDataRow(ws, row++, new object[] { "Tiền xét nghiệm", reportData.Summary.TotalTestCount, reportData.Summary.TotalTestFeeRevenue }, new[] { 3 });
            CreateDataRow(ws, row++, new object[] { "Số công tiêm", reportData.Summary.TotalInjectionCount, reportData.Summary.TotalInjectionRevenue }, new[] { 3 });

            // Total row
            CreateTotalRow(ws, row, new object[]
            {
                "TỔNG CỘNG",
                reportData.Summary.TotalExamCount + reportData.Summary.TotalTestCount + reportData.Summary.TotalInjectionCount,
                reportData.Summary.TotalRevenue
            }, 3, new[] { 3 });

            row += 3;

            // Extra stats
            ws.Cells[row, 1].Value = "Doanh thu trung bình/ngày:";
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 2].Value = reportData.Summary.AverageDailyRevenue;
            ApplyCurrencyFormat(ws.Cells[row, 2]);

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        private void CreateDailyRevenueSheet(ExcelPackage package, HospitalRevenueReportDTO reportData)
        {
            var ws = package.Workbook.Worksheets.Add("Chi tiết theo ngày");

            // Title
            ws.Cells[1, 1, 1, 8].Merge = true;
            ws.Cells[1, 1].Value = "CHI TIẾT DOANH THU THEO NGÀY";
            ws.Cells[1, 1].Style.Font.Size = 14;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Header
            int headerRow = 3;
            string[] headers = {
                "Ngày",
                "SL Công khám", "Tiền khám (VNĐ)",
                "SL XN", "Tiền Xét nghiệm (VNĐ)",
                "SL Công tiêm", "Tiền tiêm (VNĐ)",
                "Tổng doanh thu (VNĐ)"
            };
            CreateHeaderRow(ws, headerRow, headers, Color.LightGray);

            // Data
            int dataRow = headerRow + 1;
            foreach (var d in reportData.DailyRevenues.OrderBy(x => x.Date))
            {
                CreateDataRow(ws, dataRow++, new object[]
                {
                    d.Date.ToString("dd/MM/yyyy"),
                    d.ExamCount, d.ExamFeeRevenue,
                    d.TestCount, d.TestFeeRevenue,
                    d.InjectionCount, d.InjectionRevenue,
                    d.TotalRevenue
                }, new[] { 3, 5, 7, 8 }, boldCol: 8);
            }

            // Total row
            if (reportData.DailyRevenues.Any())
            {
                CreateTotalRow(ws, dataRow, new object[]
                {
                    "TỔNG CỘNG",
                    reportData.Summary.TotalExamCount, reportData.Summary.TotalExamFeeRevenue,
                    reportData.Summary.TotalTestCount, reportData.Summary.TotalTestFeeRevenue,
                    reportData.Summary.TotalInjectionCount, reportData.Summary.TotalInjectionRevenue,
                    reportData.Summary.TotalRevenue
                }, headers.Length, new[] { 3, 5, 7, 8 });
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }
    }
}
