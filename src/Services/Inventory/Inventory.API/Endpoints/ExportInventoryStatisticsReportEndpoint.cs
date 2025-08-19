using Inventory.Application.Reports;
using Inventory.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Endpoints
{
    public class ExportInventoryStatisticsReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/inventory-statistics/export", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                string? vaccineCategory,
                ISender sender,
                [FromServices]
                IInventoryStatisticsExcelService excelService) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var query = new GetInventoryStatisticsReportQuery(fromDate, toDate, vaccineCategory);
                var reportData = await sender.Send(query);

                var excelBytes = await excelService.GenerateExcelReportAsync(reportData);

                var fileName = $"BaoCaoThongKeKhoVaccine_{reportData.FromDate:yyyyMMdd}_{reportData.ToDate:yyyyMMdd}.xlsx";

                return Results.File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            })
            .RequireAuthorization()
            .WithName("ExportInventoryStatisticsReport")
            .WithSummary("Export inventory statistics report to Excel")
            .WithDescription("Export comprehensive inventory statistics report as Excel file")
            .Produces<FileResult>()
            .ProducesProblem(400)
            .ProducesProblem(500);
        }
    }
}