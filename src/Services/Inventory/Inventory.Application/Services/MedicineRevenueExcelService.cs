using BuildingBlocks.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Inventory.Application.Services
{
    public class MedicineRevenueExcelService : IMedicineRevenueExcelService
    {
        public async Task<byte[]> GenerateExcelReportAsync(MedicineRevenueReportDTO reportData)
        {
            using var package = new ExcelPackage();

            // Main report sheet
            CreateMainReportSheet(package, reportData);

            // Category analysis sheet
            CreateCategoryAnalysisSheet(package, reportData);

            // Daily statistics sheet
            CreateDailyStatisticsSheet(package, reportData);

            // Batch details sheet
            CreateBatchDetailsSheet(package, reportData);

            return await package.GetAsByteArrayAsync();
        }

        private void CreateMainReportSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Báo cáo chính");

            // Report header
            ExcelHelper.CreateReportHeader(worksheet, "BÁO CÁO DOANH SỐ SỬ DỤNG THUỐC",
                reportData.FromDate, reportData.ToDate, reportData.GeneratedAt, 9);

            // Summary section
            int currentRow = 5;
            ExcelHelper.CreateSectionTitle(worksheet, currentRow, 9, "TỔNG QUAN", Color.LightGray);

            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "• Tổng doanh thu:";
            worksheet.Cells[currentRow, 2].Value = reportData.Summary.TotalRevenue;
            ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 2]);
            worksheet.Cells[currentRow, 5].Value = "• Tổng số loại thuốc:";
            worksheet.Cells[currentRow, 6].Value = reportData.Summary.TotalMedicineTypes;

            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "• Tổng số lượng sử dụng:";
            worksheet.Cells[currentRow, 2].Value = reportData.Summary.TotalQuantityUsed;
            worksheet.Cells[currentRow, 5].Value = "• Đơn giá trung bình:";
            worksheet.Cells[currentRow, 6].Value = reportData.Summary.AverageUnitPrice;
            ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 6]);

            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "• Lợi nhuận ước tính:";
            worksheet.Cells[currentRow, 2].Value = reportData.Summary.EstimatedProfit;
            ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 2]);

            // Main data table
            currentRow += 2;
            ExcelHelper.CreateSectionTitle(worksheet, currentRow, 9, "CHI TIẾT THEO THUỐC", Color.LightBlue);

            currentRow++;
            // Headers
            string[] headers = { "STT", "Mã thuốc", "Tên thuốc", "Đơn vị", "Phân loại", "SL dùng", "Đơn giá TB", "Doanh thu", "Nhà cung cấp" };
            ExcelHelper.CreateHeaderRow(worksheet, currentRow, headers, Color.LightGray);

            // Data rows
            currentRow++;
            foreach (var medicine in reportData.MedicineDetails)
            {
                ExcelHelper.CreateDataRow(worksheet, currentRow, new object[]
                {
                    medicine.Stt,
                    medicine.MedicineCode,
                    medicine.MedicineName,
                    medicine.Unit,
                    medicine.Classification,
                    medicine.QuantityUsed,
                    medicine.AverageUnitPrice,
                    medicine.TotalRevenue,
                    medicine.SupplierName
                }, new[] { 7, 8 }, new[] { 6 });

                currentRow++;
            }

            // Total row
            ExcelHelper.CreateTotalRow(worksheet, currentRow, new object[]
            {
                "TỔNG CỘNG:", "", "", "", "",
                reportData.Summary.TotalQuantityUsed,
                "",
                reportData.Summary.TotalRevenue,
                ""
            }, 9, new[] { 8 }, new[] { 6 });

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            // Freeze panes
            worksheet.View.FreezePanes(currentRow - reportData.MedicineDetails.Count, 1);
        }

        private void CreateCategoryAnalysisSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Phân tích theo loại");

            // Report header
            ExcelHelper.CreateReportHeader(worksheet, "PHÂN TÍCH DOANH SỐ THEO PHÂN LOẠI THUỐC",
                reportData.FromDate, reportData.ToDate, reportData.GeneratedAt, 6);

            // Column headers
            int currentRow = 4;
            string[] headers = { "Phân loại", "Số lượng", "Doanh thu", "Tỷ lệ %", "Lợi nhuận ước tính", "Tỷ lệ lợi nhuận" };
            ExcelHelper.CreateHeaderRow(worksheet, currentRow, headers, Color.LightGray);

            // Data rows
            currentRow++;
            foreach (var category in reportData.CategoryStatistics)
            {
                ExcelHelper.CreateDataRow(worksheet, currentRow, new object[]
                {
                    category.Category,
                    category.Quantity,
                    category.Revenue,
                    category.Percentage / 100,
                    category.EstimatedProfit,
                    category.ProfitMargin / 100
                }, new[] { 3, 5 }, null, new[] { 4, 6 });

                currentRow++;
            }

            // Total row
            var totalRevenue = reportData.CategoryStatistics.Sum(c => c.Revenue);
            var totalQuantity = reportData.CategoryStatistics.Sum(c => c.Quantity);
            var totalProfit = reportData.CategoryStatistics.Sum(c => c.EstimatedProfit);

            ExcelHelper.CreateTotalRow(worksheet, currentRow, new object[]
            {
                "TỔNG CỘNG:",
                totalQuantity,
                totalRevenue,
                1.0,
                totalProfit,
                totalRevenue > 0 ? totalProfit / totalRevenue : 0
            }, 6, new[] { 3, 5 }, null, new[] { 4, 6 });

            worksheet.Cells.AutoFitColumns();
        }

        private void CreateDailyStatisticsSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Thống kê theo ngày");

            // Report header
            ExcelHelper.CreateReportHeader(worksheet, "THỐNG KÊ DOANH SỐ THEO NGÀY",
                reportData.FromDate, reportData.ToDate, reportData.GeneratedAt, 4);

            // Column headers
            int currentRow = 4;
            string[] headers = { "Ngày", "SL sử dụng", "Doanh thu", "Số loại thuốc" };
            ExcelHelper.CreateHeaderRow(worksheet, currentRow, headers, Color.LightGray);

            // Data rows
            currentRow++;
            decimal totalDailyRevenue = 0;
            int totalDailyQuantity = 0;

            foreach (var daily in reportData.DailyStatistics)
            {
                ExcelHelper.CreateDataRow(worksheet, currentRow, new object[]
                {
                    daily.Date,
                    daily.QuantityUsed,
                    daily.Revenue,
                    daily.MedicineTypeCount
                }, new[] { 3 }, new[] { 2, 4 });

                worksheet.Cells[currentRow, 1].Style.Numberformat.Format = "dd/MM/yyyy";

                totalDailyRevenue += daily.Revenue;
                totalDailyQuantity += daily.QuantityUsed;

                currentRow++;
            }

            // Total row
            ExcelHelper.CreateTotalRow(worksheet, currentRow, new object[]
            {
                "TỔNG CỘNG:",
                totalDailyQuantity,
                totalDailyRevenue,
                ""
            }, 4, new[] { 3 }, new[] { 2 });

            // Add average row
            currentRow++;
            var avgQuantity = reportData.DailyStatistics.Any() ? totalDailyQuantity / reportData.DailyStatistics.Count : 0;
            var avgRevenue = reportData.DailyStatistics.Any() ? totalDailyRevenue / reportData.DailyStatistics.Count : 0;

            worksheet.Cells[currentRow, 1].Value = "TRUNG BÌNH/NGÀY:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = avgQuantity;
            worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Value = avgRevenue;
            worksheet.Cells[currentRow, 3].Style.Font.Bold = true;
            ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 3]);

            worksheet.Cells.AutoFitColumns();
        }

        private void CreateBatchDetailsSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Chi tiết lô thuốc");

            // Report header
            ExcelHelper.CreateReportHeader(worksheet, "CHI TIẾT SỬ DỤNG THEO LÔ THUỐC",
                reportData.FromDate, reportData.ToDate, reportData.GeneratedAt, 9);

            // Column headers
            int currentRow = 4;
            string[] headers = { "Mã thuốc", "Tên thuốc", "Số lô", "Hạn sử dụng", "SL sử dụng", "Giá nhập", "Giá bán", "Doanh thu", "Lợi nhuận" };
            ExcelHelper.CreateHeaderRow(worksheet, currentRow, headers, Color.LightGray);

            // Group by medicine for better organization
            var groupedBatches = reportData.BatchDetails
                .GroupBy(b => new { b.MedicineCode, b.MedicineName })
                .OrderBy(g => g.Key.MedicineName);

            currentRow++;
            foreach (var medicineGroup in groupedBatches)
            {
                // Medicine group header
                ExcelHelper.CreateSectionTitle(worksheet, currentRow, 9,
                    $"THUỐC: {medicineGroup.Key.MedicineName} ({medicineGroup.Key.MedicineCode})", Color.LightYellow);
                currentRow++;

                // Batch details for this medicine
                foreach (var batch in medicineGroup.OrderBy(b => b.BatchNumber))
                {
                    ExcelHelper.CreateDataRow(worksheet, currentRow, new object[]
                    {
                        batch.MedicineCode,
                        batch.MedicineName,
                        batch.BatchNumber,
                        batch.ExpiryDate.ToString("dd/MM/yyyy"),
                        batch.QuantityUsed,
                        batch.ImportPrice,
                        batch.SellingPrice,
                        batch.Revenue,
                        batch.Profit
                    }, new[] { 6, 7, 8, 9 }, new[] { 5 });

                    // Color coding for profit
                    if (batch.Profit > 0)
                    {
                        worksheet.Cells[currentRow, 9].Style.Font.Color.SetColor(Color.Green);
                    }
                    else if (batch.Profit < 0)
                    {
                        worksheet.Cells[currentRow, 9].Style.Font.Color.SetColor(Color.Red);
                    }

                    currentRow++;
                }

                // Subtotal for this medicine group
                var groupTotal = medicineGroup.Sum(b => b.Revenue);
                var groupProfit = medicineGroup.Sum(b => b.Profit);
                var groupQuantity = medicineGroup.Sum(b => b.QuantityUsed);

                worksheet.Cells[currentRow, 1, currentRow, 4].Merge = true;
                worksheet.Cells[currentRow, 1].Value = $"Tổng {medicineGroup.Key.MedicineName}:";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);

                worksheet.Cells[currentRow, 5].Value = groupQuantity;
                worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 8].Value = groupTotal;
                worksheet.Cells[currentRow, 8].Style.Font.Bold = true;
                ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 8]);
                worksheet.Cells[currentRow, 9].Value = groupProfit;
                worksheet.Cells[currentRow, 9].Style.Font.Bold = true;
                ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 9]);

                currentRow += 2; // Add space between medicine groups
            }

            // Grand total
            ExcelHelper.CreateSectionTitle(worksheet, currentRow, 9, "TỔNG CỘNG TẤT CẢ:", Color.Orange);

            worksheet.Cells[currentRow, 5].Value = reportData.BatchDetails.Sum(b => b.QuantityUsed);
            worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 8].Value = reportData.BatchDetails.Sum(b => b.Revenue);
            worksheet.Cells[currentRow, 8].Style.Font.Bold = true;
            ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 8]);
            worksheet.Cells[currentRow, 9].Value = reportData.BatchDetails.Sum(b => b.Profit);
            worksheet.Cells[currentRow, 9].Style.Font.Bold = true;
            ExcelHelper.ApplyCurrencyFormat(worksheet.Cells[currentRow, 9]);

            worksheet.Cells.AutoFitColumns();

            // Freeze panes at header
            worksheet.View.FreezePanes(5, 1);
        }
    }
}