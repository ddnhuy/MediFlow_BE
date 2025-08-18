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

            return await Task.FromResult(package.GetAsByteArray());
        }

        private void CreateMainReportSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Báo cáo chính");

            // Header
            worksheet.Cells[1, 1, 1, 9].Merge = true;
            worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH SỐ SỬ DỤNG THUỐC";
            worksheet.Cells[1, 1].Style.Font.Size = 16;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1, 2, 9].Merge = true;
            worksheet.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[3, 1, 3, 9].Merge = true;
            worksheet.Cells[3, 1].Value = $"Ngày xuất: {reportData.GeneratedAt:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Summary section
            int currentRow = 5;
            worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;
            worksheet.Cells[currentRow, 1].Value = "TỔNG QUAN";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "• Tổng doanh thu:";
            worksheet.Cells[currentRow, 2].Value = reportData.Summary.TotalRevenue;
            worksheet.Cells[currentRow, 2].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[currentRow, 5].Value = "• Tổng số loại thuốc:";
            worksheet.Cells[currentRow, 6].Value = reportData.Summary.TotalMedicineTypes;

            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "• Tổng số lượng sử dụng:";
            worksheet.Cells[currentRow, 2].Value = reportData.Summary.TotalQuantityUsed;
            worksheet.Cells[currentRow, 5].Value = "• Đơn giá trung bình:";
            worksheet.Cells[currentRow, 6].Value = reportData.Summary.AverageUnitPrice;
            worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0 ₫";

            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "• Lợi nhuận ước tính:";
            worksheet.Cells[currentRow, 2].Value = reportData.Summary.EstimatedProfit;
            worksheet.Cells[currentRow, 2].Style.Numberformat.Format = "#,##0 ₫";

            // Main data table
            currentRow += 2;
            worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;
            worksheet.Cells[currentRow, 1].Value = "CHI TIẾT THEO THUỐC";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

            currentRow++;
            // Headers
            string[] headers = { "STT", "Mã thuốc", "Tên thuốc", "Đơn vị", "Phân loại", "SL dùng", "Đơn giá TB", "Doanh thu", "Nhà cung cấp" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1].Value = headers[i];
                worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Data rows
            currentRow++;
            foreach (var medicine in reportData.MedicineDetails)
            {
                worksheet.Cells[currentRow, 1].Value = medicine.Stt;
                worksheet.Cells[currentRow, 2].Value = medicine.MedicineCode;
                worksheet.Cells[currentRow, 3].Value = medicine.MedicineName;
                worksheet.Cells[currentRow, 4].Value = medicine.Unit;
                worksheet.Cells[currentRow, 5].Value = medicine.Classification;
                worksheet.Cells[currentRow, 6].Value = medicine.QuantityUsed;
                worksheet.Cells[currentRow, 7].Value = medicine.AverageUnitPrice;
                worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[currentRow, 8].Value = medicine.TotalRevenue;
                worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[currentRow, 9].Value = medicine.SupplierName;

                // Apply borders
                for (int col = 1; col <= 9; col++)
                {
                    worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                currentRow++;
            }

            // Total row
            worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
            worksheet.Cells[currentRow, 1].Value = "TỔNG CỘNG:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 6].Value = reportData.Summary.TotalQuantityUsed;
            worksheet.Cells[currentRow, 6].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 8].Value = reportData.Summary.TotalRevenue;
            worksheet.Cells[currentRow, 8].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0";

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            // Freeze panes
            worksheet.View.FreezePanes(currentRow - reportData.MedicineDetails.Count, 1);
        }

        private void CreateCategoryAnalysisSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Phân tích theo loại");

            // Header
            worksheet.Cells[1, 1, 1, 6].Merge = true;
            worksheet.Cells[1, 1].Value = "PHÂN TÍCH DOANH SỐ THEO PHÂN LOẠI THUỐC";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1, 2, 6].Merge = true;
            worksheet.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int currentRow = 4;
            string[] headers = { "Phân loại", "Số lượng", "Doanh thu", "Tỷ lệ %", "Lợi nhuận ước tính", "Tỷ lệ lợi nhuận" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1].Value = headers[i];
                worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Data rows
            currentRow++;
            foreach (var category in reportData.CategoryStatistics)
            {
                worksheet.Cells[currentRow, 1].Value = category.Category;
                worksheet.Cells[currentRow, 2].Value = category.Quantity;
                worksheet.Cells[currentRow, 3].Value = category.Revenue;
                worksheet.Cells[currentRow, 3].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[currentRow, 4].Value = category.Percentage / 100; // Convert to decimal for percentage format
                worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "0.0%";
                worksheet.Cells[currentRow, 5].Value = category.EstimatedProfit;
                worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[currentRow, 6].Value = category.ProfitMargin / 100; // Convert to decimal for percentage format
                worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "0.0%";

                // Apply borders
                for (int col = 1; col <= 6; col++)
                {
                    worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                currentRow++;
            }

            // Total row
            var totalRevenue = reportData.CategoryStatistics.Sum(c => c.Revenue);
            var totalQuantity = reportData.CategoryStatistics.Sum(c => c.Quantity);
            var totalProfit = reportData.CategoryStatistics.Sum(c => c.EstimatedProfit);

            worksheet.Cells[currentRow, 1].Value = "TỔNG CỘNG:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = totalQuantity;
            worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Value = totalRevenue;
            worksheet.Cells[currentRow, 3].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[currentRow, 4].Value = 1.0; // 100%
            worksheet.Cells[currentRow, 4].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "0.0%";
            worksheet.Cells[currentRow, 5].Value = totalProfit;
            worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[currentRow, 6].Value = totalRevenue > 0 ? totalProfit / totalRevenue : 0;
            worksheet.Cells[currentRow, 6].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "0.0%";

            worksheet.Cells.AutoFitColumns();
        }

        private void CreateDailyStatisticsSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Thống kê theo ngày");

            // Header
            worksheet.Cells[1, 1, 1, 4].Merge = true;
            worksheet.Cells[1, 1].Value = "THỐNG KÊ DOANH SỐ THEO NGÀY";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1, 2, 4].Merge = true;
            worksheet.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int currentRow = 4;
            string[] headers = { "Ngày", "SL sử dụng", "Doanh thu", "Số loại thuốc" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1].Value = headers[i];
                worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Data rows
            currentRow++;
            decimal totalDailyRevenue = 0;
            int totalDailyQuantity = 0;

            foreach (var daily in reportData.DailyStatistics)
            {
                worksheet.Cells[currentRow, 1].Value = daily.Date;
                worksheet.Cells[currentRow, 1].Style.Numberformat.Format = "dd/MM/yyyy";
                worksheet.Cells[currentRow, 2].Value = daily.QuantityUsed;
                worksheet.Cells[currentRow, 3].Value = daily.Revenue;
                worksheet.Cells[currentRow, 3].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[currentRow, 4].Value = daily.MedicineTypeCount;

                totalDailyRevenue += daily.Revenue;
                totalDailyQuantity += daily.QuantityUsed;

                // Apply borders
                for (int col = 1; col <= 4; col++)
                {
                    worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                currentRow++;
            }

            // Total row
            worksheet.Cells[currentRow, 1].Value = "TỔNG CỘNG:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = totalDailyQuantity;
            worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Value = totalDailyRevenue;
            worksheet.Cells[currentRow, 3].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Style.Numberformat.Format = "#,##0 ₫";

            // Add average row
            currentRow++;
            worksheet.Cells[currentRow, 1].Value = "TRUNG BÌNH/NGÀY:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            var avgQuantity = reportData.DailyStatistics.Any() ? totalDailyQuantity / reportData.DailyStatistics.Count : 0;
            var avgRevenue = reportData.DailyStatistics.Any() ? totalDailyRevenue / reportData.DailyStatistics.Count : 0;
            worksheet.Cells[currentRow, 2].Value = avgQuantity;
            worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Value = avgRevenue;
            worksheet.Cells[currentRow, 3].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 3].Style.Numberformat.Format = "#,##0 ₫";

            worksheet.Cells.AutoFitColumns();
        }

        private void CreateBatchDetailsSheet(ExcelPackage package, MedicineRevenueReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Chi tiết lô thuốc");

            // Header
            worksheet.Cells[1, 1, 1, 9].Merge = true;
            worksheet.Cells[1, 1].Value = "CHI TIẾT SỬ DỤNG THEO LÔ THUỐC";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[2, 1, 2, 9].Merge = true;
            worksheet.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int currentRow = 4;
            string[] headers = { "Mã thuốc", "Tên thuốc", "Số lô", "Hạn sử dụng", "SL sử dụng", "Giá nhập", "Giá bán", "Doanh thu", "Lợi nhuận" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[currentRow, i + 1].Value = headers[i];
                worksheet.Cells[currentRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Group by medicine for better organization
            var groupedBatches = reportData.BatchDetails
                .GroupBy(b => new { b.MedicineCode, b.MedicineName })
                .OrderBy(g => g.Key.MedicineName);

            currentRow++;
            foreach (var medicineGroup in groupedBatches)
            {
                // Medicine group header
                worksheet.Cells[currentRow, 1, currentRow, 9].Merge = true;
                worksheet.Cells[currentRow, 1].Value = $"THUỐC: {medicineGroup.Key.MedicineName} ({medicineGroup.Key.MedicineCode})";
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                currentRow++;

                // Batch details for this medicine
                foreach (var batch in medicineGroup.OrderBy(b => b.BatchNumber))
                {
                    worksheet.Cells[currentRow, 1].Value = batch.MedicineCode;
                    worksheet.Cells[currentRow, 2].Value = batch.MedicineName;
                    worksheet.Cells[currentRow, 3].Value = batch.BatchNumber;
                    worksheet.Cells[currentRow, 4].Value = batch.ExpiryDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[currentRow, 5].Value = batch.QuantityUsed;
                    worksheet.Cells[currentRow, 6].Value = batch.ImportPrice;
                    worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0 ₫";
                    worksheet.Cells[currentRow, 7].Value = batch.SellingPrice;
                    worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0 ₫";
                    worksheet.Cells[currentRow, 8].Value = batch.Revenue;
                    worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0 ₫";
                    worksheet.Cells[currentRow, 9].Value = batch.Profit;
                    worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0 ₫";

                    // Color coding for profit
                    if (batch.Profit > 0)
                    {
                        worksheet.Cells[currentRow, 9].Style.Font.Color.SetColor(Color.Green);
                    }
                    else if (batch.Profit < 0)
                    {
                        worksheet.Cells[currentRow, 9].Style.Font.Color.SetColor(Color.Red);
                    }

                    // Apply borders
                    for (int col = 1; col <= 9; col++)
                    {
                        worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
                worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[currentRow, 9].Value = groupProfit;
                worksheet.Cells[currentRow, 9].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0 ₫";

                currentRow += 2; // Add space between medicine groups
            }

            // Grand total
            worksheet.Cells[currentRow, 1, currentRow, 4].Merge = true;
            worksheet.Cells[currentRow, 1].Value = "TỔNG CỘNG TẤT CẢ:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.Orange);

            worksheet.Cells[currentRow, 5].Value = reportData.BatchDetails.Sum(b => b.QuantityUsed);
            worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 8].Value = reportData.BatchDetails.Sum(b => b.Revenue);
            worksheet.Cells[currentRow, 8].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[currentRow, 9].Value = reportData.BatchDetails.Sum(b => b.Profit);
            worksheet.Cells[currentRow, 9].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0 ₫";

            worksheet.Cells.AutoFitColumns();

            // Freeze panes at header
            worksheet.View.FreezePanes(5, 1);
        }
    }
}
