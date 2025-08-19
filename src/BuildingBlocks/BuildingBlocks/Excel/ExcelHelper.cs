using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BuildingBlocks.Excel
{
    public static class ExcelHelper
    {
        public static void ApplyHeaderStyle(ExcelRange cell, Color bgColor)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(bgColor);
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        public static void ApplyCellBorder(ExcelRange cell) =>
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

        public static void ApplyCurrencyFormat(ExcelRange cell) =>
            cell.Style.Numberformat.Format = "#,##0 ₫";

        public static void ApplyNumberFormat(ExcelRange cell) =>
            cell.Style.Numberformat.Format = "#,##0";

        public static void ApplyPercentageFormat(ExcelRange cell) =>
            cell.Style.Numberformat.Format = "0.0%";

        public static void CreateHeaderRow(ExcelWorksheet ws, int row, string[] headers, Color bgColor)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[row, i + 1];
                cell.Value = headers[i];
                ApplyHeaderStyle(cell, bgColor);
            }
        }

        public static void CreateDataRow(
            ExcelWorksheet ws,
            int row,
            object[] values,
            int[] currencyCols = null,
            int[] numberCols = null,
            int[] percentageCols = null,
            int? boldCol = null)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var cell = ws.Cells[row, i + 1];
                cell.Value = values[i];
                ApplyCellBorder(cell);

                if (currencyCols != null && currencyCols.Contains(i + 1))
                    ApplyCurrencyFormat(cell);

                if (numberCols != null && numberCols.Contains(i + 1))
                    ApplyNumberFormat(cell);

                if (percentageCols != null && percentageCols.Contains(i + 1))
                    ApplyPercentageFormat(cell);

                if (boldCol.HasValue && (i + 1) == boldCol.Value)
                    cell.Style.Font.Bold = true;
            }
        }

        public static void CreateTotalRow(
            ExcelWorksheet ws,
            int row,
            object[] values,
            int colCount,
            int[] currencyCols = null,
            int[] numberCols = null,
            int[] percentageCols = null)
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

                if (numberCols != null && numberCols.Contains(i + 1))
                    ApplyNumberFormat(cell);

                if (percentageCols != null && percentageCols.Contains(i + 1))
                    ApplyPercentageFormat(cell);
            }
        }

        public static void CreateSectionTitle(ExcelWorksheet ws, int row, int colCount, string title, Color bgColor)
        {
            ws.Cells[row, 1, row, colCount].Merge = true;
            ws.Cells[row, 1].Value = title;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(bgColor);
        }

        public static void CreateReportHeader(
            ExcelWorksheet ws,
            string title,
            DateOnly fromDate,
            DateOnly toDate,
            DateTime generatedAt,
            int colCount,
            string generatedBy = null)
        {
            ws.Cells[1, 1, 1, colCount].Merge = true;
            ws.Cells[1, 1].Value = title;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 16;
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[2, 1, 2, colCount].Merge = true;
            ws.Cells[2, 1].Value = $"Từ ngày: {fromDate:dd/MM/yyyy} - Đến ngày: {toDate:dd/MM/yyyy}";
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Cells[3, 1, 3, colCount].Merge = true;
            ws.Cells[3, 1].Value = !string.IsNullOrEmpty(generatedBy)
                ? $"Ngày xuất: {generatedAt:dd/MM/yyyy HH:mm:ss} - Người xuất: {generatedBy}"
                : $"Ngày xuất: {generatedAt:dd/MM/yyyy HH:mm:ss}";
            ws.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }
}