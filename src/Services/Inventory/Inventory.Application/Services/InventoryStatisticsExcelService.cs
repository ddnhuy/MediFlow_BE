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

            CreateStatisticsSheet(package, reportData);
            CreateVaccineStocksSheet(package, reportData);
            CreateBatchDetailsSheet(package, reportData);
            CreateTransactionsSheet(package, reportData);

            return await package.GetAsByteArrayAsync();
        }

        #region Helpers
        private void CreateMergedHeader(ExcelWorksheet sheet, int fromCol, int toCol, string text, int fontSize = 14, bool bold = true)
        {
            sheet.Cells[1, fromCol, 1, toCol].Merge = true;
            sheet.Cells[1, fromCol].Value = text;
            sheet.Cells[1, fromCol].Style.Font.Size = fontSize;
            sheet.Cells[1, fromCol].Style.Font.Bold = bold;
            sheet.Cells[1, fromCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private void CreateColumnHeaders(ExcelWorksheet sheet, int row, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[row, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        private void ApplyBorders(ExcelWorksheet sheet, int row, int colCount)
        {
            for (int col = 1; col <= colCount; col++)
            {
                sheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }
        }

        private void AutoFit(ExcelWorksheet sheet)
        {
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        }
        #endregion

        private void CreateStatisticsSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var sheet = package.Workbook.Worksheets.Add("Thống kê tổng quan");

            // Title + meta
            CreateMergedHeader(sheet, 1, 6, "BÁO CÁO THỐNG KÊ KHO VACCINE", 16);

            sheet.Cells[2, 1, 2, 6].Merge = true;
            sheet.Cells[2, 1].Value = $"Từ ngày: {reportData.FromDate:dd/MM/yyyy} - Đến ngày: {reportData.ToDate:dd/MM/yyyy}";
            sheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[3, 1, 3, 6].Merge = true;
            sheet.Cells[3, 1].Value = $"Ngày xuất: {reportData.GeneratedAt:dd/MM/yyyy HH:mm:ss} - Người xuất: {reportData.GeneratedBy}";
            sheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Summary
            int row = 5;
            sheet.Cells[row, 1].Value = "THỐNG KÊ TỔNG QUAN";
            sheet.Cells[row, 1].Style.Font.Size = 14;
            sheet.Cells[row, 1].Style.Font.Bold = true;

            void WriteSummary(string label, object value, string? format = null, Color? color = null)
            {
                row++;
                sheet.Cells[row, 1].Value = label;
                sheet.Cells[row, 1].Style.Font.Bold = true;
                sheet.Cells[row, 2].Value = value;
                if (!string.IsNullOrEmpty(format))
                    sheet.Cells[row, 2].Style.Numberformat.Format = format;
                if (color.HasValue)
                    sheet.Cells[row, 2].Style.Font.Color.SetColor(color.Value);
            }

            WriteSummary("Tổng số loại vaccine:", reportData.Summary.TotalVaccineTypes);
            WriteSummary("Tổng số lượng tồn kho:", reportData.Summary.TotalQuantityInStock, "#,##0");
            WriteSummary("Tổng giá trị tồn kho:", reportData.Summary.TotalInventoryValue, "#,##0 ₫");
            WriteSummary("Tổng số lô hàng:", reportData.Summary.TotalBatches);
            WriteSummary("Số lô gần hết hạn:", reportData.Summary.BatchesNearExpiry,
                null, reportData.Summary.BatchesNearExpiry > 0 ? Color.Orange : null);
            WriteSummary("Số vaccine tồn kho thấp:", reportData.Summary.LowStockVaccines,
                null, reportData.Summary.LowStockVaccines > 0 ? Color.Red : null);

            AutoFit(sheet);
        }

        private void CreateVaccineStocksSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var sheet = package.Workbook.Worksheets.Add("Tồn kho vaccine");

            CreateMergedHeader(sheet, 1, 9, "THỐNG KÊ TỒN KHO THEO LOẠI VACCINE");

            int headerRow = 3;
            string[] headers = { "STT", "Mã vaccine", "Tên vaccine", "Đơn vị", "Phân loại", "Tổng SL", "Giá TB", "Tổng giá trị", "Trạng thái" };
            CreateColumnHeaders(sheet, headerRow, headers);

            int row = headerRow + 1;
            foreach (var v in reportData.VaccineStocks)
            {
                sheet.Cells[row, 1].Value = v.Stt;
                sheet.Cells[row, 2].Value = v.VaccineCode;
                sheet.Cells[row, 3].Value = v.VaccineName;
                sheet.Cells[row, 4].Value = v.Unit;
                sheet.Cells[row, 5].Value = v.Classification;
                sheet.Cells[row, 6].Value = v.TotalQuantity;
                sheet.Cells[row, 7].Value = v.AverageUnitPrice;
                sheet.Cells[row, 8].Value = v.TotalValue;
                sheet.Cells[row, 9].Value = v.Status;

                sheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";
                sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0 ₫";
                sheet.Cells[row, 8].Style.Numberformat.Format = "#,##0 ₫";

                if (v.Status.Contains("thấp") || v.Status.Contains("nghiêm trọng"))
                    sheet.Cells[row, 9].Style.Font.Color.SetColor(Color.Red);
                else if (v.Status.Contains("hạn"))
                    sheet.Cells[row, 9].Style.Font.Color.SetColor(Color.Orange);

                ApplyBorders(sheet, row, headers.Length);
                row++;
            }

            AutoFit(sheet);
        }

        private void CreateBatchDetailsSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var sheet = package.Workbook.Worksheets.Add("Chi tiết lô hàng");

            CreateMergedHeader(sheet, 1, 10, "CHI TIẾT CÁC LÔ HÀNG VACCINE");

            int headerRow = 3;
            string[] headers = { "STT", "Mã vaccine", "Tên vaccine", "Số lô", "Nhà cung cấp", "Số lượng", "Đơn giá", "Tổng giá trị", "Ngày hết hạn", "Trạng thái" };
            CreateColumnHeaders(sheet, headerRow, headers);

            int row = headerRow + 1;
            foreach (var b in reportData.BatchDetails)
            {
                sheet.Cells[row, 1].Value = b.Stt;
                sheet.Cells[row, 2].Value = b.VaccineCode;
                sheet.Cells[row, 3].Value = b.VaccineName;
                sheet.Cells[row, 4].Value = b.BatchNumber;
                sheet.Cells[row, 5].Value = b.SupplierName;
                sheet.Cells[row, 6].Value = b.Quantity;
                sheet.Cells[row, 7].Value = b.UnitPrice;
                sheet.Cells[row, 8].Value = b.TotalValue;
                sheet.Cells[row, 9].Value = b.ExpiryDate.ToString("dd/MM/yyyy");
                sheet.Cells[row, 10].Value = b.Status;

                sheet.Cells[row, 6].Style.Numberformat.Format = "#,##0";
                sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0 ₫";
                sheet.Cells[row, 8].Style.Numberformat.Format = "#,##0 ₫";

                if (b.Status.Contains("Hết hạn"))
                {
                    sheet.Cells[row, 10].Style.Font.Color.SetColor(Color.Red);
                    sheet.Cells[row, 10].Style.Font.Bold = true;
                }
                else if (b.Status.Contains("hạn"))
                {
                    sheet.Cells[row, 10].Style.Font.Color.SetColor(Color.Orange);
                }

                ApplyBorders(sheet, row, headers.Length);
                row++;
            }

            AutoFit(sheet);
        }

        private void CreateTransactionsSheet(ExcelPackage package, InventoryStatisticsReportDTO reportData)
        {
            var sheet = package.Workbook.Worksheets.Add("Lịch sử giao dịch");

            CreateMergedHeader(sheet, 1, 9, "LỊCH SỬ GIAO DỊCH KHO VACCINE");

            int headerRow = 3;
            string[] headers = { "STT", "Ngày GD", "Loại GD", "Mã vaccine", "Tên vaccine", "Số lô", "Số lượng", "Đơn giá", "Tổng tiền" };
            CreateColumnHeaders(sheet, headerRow, headers);

            int row = headerRow + 1;
            foreach (var t in reportData.Transactions)
            {
                sheet.Cells[row, 1].Value = t.Stt;
                sheet.Cells[row, 2].Value = t.TransactionDate.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[row, 3].Value = t.TransactionType;
                sheet.Cells[row, 4].Value = t.VaccineCode;
                sheet.Cells[row, 5].Value = t.VaccineName;
                sheet.Cells[row, 6].Value = t.BatchNumber;
                sheet.Cells[row, 7].Value = t.Quantity;
                sheet.Cells[row, 8].Value = t.UnitPrice;
                sheet.Cells[row, 9].Value = t.TotalValue;

                sheet.Cells[row, 7].Style.Numberformat.Format = "#,##0";
                sheet.Cells[row, 8].Style.Numberformat.Format = "#,##0 ₫";
                sheet.Cells[row, 9].Style.Numberformat.Format = "#,##0 ₫";

                if (t.TransactionType == "Nhập kho")
                    sheet.Cells[row, 3].Style.Font.Color.SetColor(Color.Green);
                else if (t.TransactionType == "Xuất kho")
                    sheet.Cells[row, 3].Style.Font.Color.SetColor(Color.Blue);

                ApplyBorders(sheet, row, headers.Length);
                row++;
            }

            AutoFit(sheet);
        }
    }
}
