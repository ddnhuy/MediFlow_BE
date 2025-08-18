using Inventory.Application.Reports;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Endpoints
{
    public class ExportMedicineRevenueReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-revenue/export", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                ISender sender,
                [FromServices]
                IMedicineRevenueExcelService excelService,
                HttpContext context) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                try
                {
                    // Get report data
                    var query = new GetMedicineRevenueReportQuery(fromDate, toDate);
                    var reportData = await sender.Send(query);

                    // Generate Excel file
                    var fileBytes = await excelService.GenerateExcelReportAsync(reportData);

                    // Generate filename
                    var fileName = $"BaoCaoDoanhSoThuoc_{reportData.FromDate:yyyyMMdd}_{reportData.ToDate:yyyyMMdd}.xlsx";

                    return Results.File(
                    fileBytes,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: fileName,
                    enableRangeProcessing: false);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Error generating Excel report");
                }
            })
            .RequireAuthorization()
            .WithName("ExportMedicineRevenueReport")
            .WithSummary("Export Medicine Revenue Report to Excel")
            .WithDescription("Exports medicine revenue report to Excel file with detailed statistics, category analysis, and batch details.")
            .Produces(StatusCodes.Status200OK, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags("Reports", "Export")
            .ExcludeFromDescription();
        }
    }
}