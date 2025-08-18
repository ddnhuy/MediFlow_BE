// src/Services/Inventory/Inventory.Application/Services/InventoryStatisticsExcelService.cs
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Inventory.Application.Services
{
    public class InventoryStatisticsExcelService : IInventoryStatisticsExcelService
    {
        public async Task<byte[]> GenerateExcelReportAsync(InventoryStatisticsReportDTO reportData)
        {
            using var package = new ExcelPackage();

            // Main statistics sheet
            CreateStatisticsSheet(package, reportData);

            // Vaccine stocks sheet
            CreateVaccineStocksSheet(package, reportData);

            // Batch details sheet
            CreateBatchDetailsSheet(package, reportData);

            // Transactions sheet
            CreateTransactionsSheet(package, reportData);

            return await package.GetAsByteArrayAsync();
        }

        private void CreateStatisticsSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Thống kê tổng quan");

            // Header
            worksheet.Cells[1, 1, 1, 6].Merge = true;
            worksheet.Cells[1, 1].Value = "BÁO CÁO THỐNG KÊ KHO VACCINE";
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
            worksheet.Cells[row, 1].Value = "Tổng số loại vaccine:";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalVaccineTypes;
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            worksheet.Cells[row, 1].Value = "Tổng số lượng tồn kho:";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalQuantityInStock;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0";
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            worksheet.Cells[row, 1].Value = "Tổng giá trị tồn kho:";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalInventoryValue;
            worksheet.Cells[row, 2].Style.Numberformat.Format = "#,##0 ₫";
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            worksheet.Cells[row, 1].Value = "Tổng số lô hàng:";
            worksheet.Cells[row, 2].Value = reportData.Summary.TotalBatches;
            worksheet.Cells[row, 1].Style.Font.Bold = true;

            row++;
            worksheet.Cells[row, 1].Value = "Số lô gần hết hạn:";
            worksheet.Cells[row, 2].Value = reportData.Summary.BatchesNearExpiry;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            if (reportData.Summary.BatchesNearExpiry > 0)
            {
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.Orange);
            }

            row++;
            worksheet.Cells[row, 1].Value = "Số vaccine tồn kho thấp:";
            worksheet.Cells[row, 2].Value = reportData.Summary.LowStockVaccines;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            if (reportData.Summary.LowStockVaccines > 0)
            {
                worksheet.Cells[row, 2].Style.Font.Color.SetColor(Color.Red);
            }

            // Auto fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateVaccineStocksSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Tồn kho vaccine");

            // Header
            worksheet.Cells[1, 1, 1, 9].Merge = true;
            worksheet.Cells[1, 1].Value = "THỐNG KÊ TỒN KHO THEO LOẠI VACCINE";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int headerRow = 3;
            string[] headers = { "STT", "Mã vaccine", "Tên vaccine", "Đơn vị", "Phân loại", "Tổng SL", "Giá TB", "Tổng giá trị", "Trạng thái" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
                worksheet.Cells[headerRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[headerRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[headerRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Data rows
            int dataRow = headerRow + 1;
            foreach (var vaccine in reportData.VaccineStocks)
            {
                worksheet.Cells[dataRow, 1].Value = vaccine.Stt;
                worksheet.Cells[dataRow, 2].Value = vaccine.VaccineCode;
                worksheet.Cells[dataRow, 3].Value = vaccine.VaccineName;
                worksheet.Cells[dataRow, 4].Value = vaccine.Unit;
                worksheet.Cells[dataRow, 5].Value = vaccine.Classification;
                worksheet.Cells[dataRow, 6].Value = vaccine.TotalQuantity;
                worksheet.Cells[dataRow, 7].Value = vaccine.AverageUnitPrice;
                worksheet.Cells[dataRow, 8].Value = vaccine.TotalValue;
                worksheet.Cells[dataRow, 9].Value = vaccine.Status;

                // Format numbers
                worksheet.Cells[dataRow, 6].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[dataRow, 7].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 8].Style.Numberformat.Format = "#,##0 ₫";

                // Color status
                if (vaccine.Status.Contains("thấp") || vaccine.Status.Contains("nghiêm trọng"))
                {
                    worksheet.Cells[dataRow, 9].Style.Font.Color.SetColor(Color.Red);
                }
                else if (vaccine.Status.Contains("hạn"))
                {
                    worksheet.Cells[dataRow, 9].Style.Font.Color.SetColor(Color.Orange);
                }

                // Borders
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                dataRow++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateBatchDetailsSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Chi tiết lô hàng");

            // Header
            worksheet.Cells[1, 1, 1, 10].Merge = true;
            worksheet.Cells[1, 1].Value = "CHI TIẾT CÁC LÔ HÀNG VACCINE";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int headerRow = 3;
            string[] headers = { "STT", "Mã vaccine", "Tên vaccine", "Số lô", "Nhà cung cấp", "Số lượng", "Đơn giá", "Tổng giá trị", "Ngày hết hạn", "Trạng thái" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
                worksheet.Cells[headerRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[headerRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[headerRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Data rows
            int dataRow = headerRow + 1;
            foreach (var batch in reportData.BatchDetails)
            {
                worksheet.Cells[dataRow, 1].Value = batch.Stt;
                worksheet.Cells[dataRow, 2].Value = batch.VaccineCode;
                worksheet.Cells[dataRow, 3].Value = batch.VaccineName;
                worksheet.Cells[dataRow, 4].Value = batch.BatchNumber;
                worksheet.Cells[dataRow, 5].Value = batch.SupplierName;
                worksheet.Cells[dataRow, 6].Value = batch.Quantity;
                worksheet.Cells[dataRow, 7].Value = batch.UnitPrice;
                worksheet.Cells[dataRow, 8].Value = batch.TotalValue;
                worksheet.Cells[dataRow, 9].Value = batch.ExpiryDate.ToString("dd/MM/yyyy");
                worksheet.Cells[dataRow, 10].Value = batch.Status;

                // Format numbers
                worksheet.Cells[dataRow, 6].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[dataRow, 7].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 8].Style.Numberformat.Format = "#,##0 ₫";

                // Color status
                if (batch.Status.Contains("Hết hạn"))
                {
                    worksheet.Cells[dataRow, 10].Style.Font.Color.SetColor(Color.Red);
                    worksheet.Cells[dataRow, 10].Style.Font.Bold = true;
                }
                else if (batch.Status.Contains("hạn"))
                {
                    worksheet.Cells[dataRow, 10].Style.Font.Color.SetColor(Color.Orange);
                }

                // Borders
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                dataRow++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateTransactionsSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var worksheet = package.Workbook.Worksheets.Add("Lịch sử giao dịch");

            // Header
            worksheet.Cells[1, 1, 1, 9].Merge = true;
            worksheet.Cells[1, 1].Value = "LỊCH SỬ GIAO DỊCH KHO VACCINE";
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Column headers
            int headerRow = 3;
            string[] headers = { "STT", "Ngày GD", "Loại GD", "Mã vaccine", "Tên vaccine", "Số lô", "Số lượng", "Đơn giá", "Tổng tiền" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
                worksheet.Cells[headerRow, i + 1].Style.Font.Bold = true;
                worksheet.Cells[headerRow, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRow, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[headerRow, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Data rows
            int dataRow = headerRow + 1;
            foreach (var transaction in reportData.Transactions)
            {
                worksheet.Cells[dataRow, 1].Value = transaction.Stt;
                worksheet.Cells[dataRow, 2].Value = transaction.TransactionDate.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[dataRow, 3].Value = transaction.TransactionType;
                worksheet.Cells[dataRow, 4].Value = transaction.VaccineCode;
                worksheet.Cells[dataRow, 5].Value = transaction.VaccineName;
                worksheet.Cells[dataRow, 6].Value = transaction.BatchNumber;
                worksheet.Cells[dataRow, 7].Value = transaction.Quantity;
                worksheet.Cells[dataRow, 8].Value = transaction.UnitPrice;
                worksheet.Cells[dataRow, 9].Value = transaction.TotalValue;

                // Format numbers
                worksheet.Cells[dataRow, 7].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[dataRow, 8].Style.Numberformat.Format = "#,##0 ₫";
                worksheet.Cells[dataRow, 9].Style.Numberformat.Format = "#,##0 ₫";

                // Color transaction type
                if (transaction.TransactionType == "Nhập kho")
                {
                    worksheet.Cells[dataRow, 3].Style.Font.Color.SetColor(Color.Green);
                }
                else if (transaction.TransactionType == "Xuất kho")
                {
                    worksheet.Cells[dataRow, 3].Style.Font.Color.SetColor(Color.Blue);
                }

                // Borders
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[dataRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                dataRow++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
    }
}