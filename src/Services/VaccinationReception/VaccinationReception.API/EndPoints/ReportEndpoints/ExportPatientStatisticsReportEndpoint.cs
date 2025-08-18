using VaccinationReception.Application.Reports;
using VaccinationReception.Application.Services;
using VaccinationReception.Application.Services.ExcelServices;

namespace VaccinationReception.API.EndPoints.Reports
{
    public class ExportPatientStatisticsReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patient-statistics/export", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                ISender sender,
                [FromServices]
                IPatientStatisticsExcelService excelService) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var query = new GetPatientStatisticsReportQuery(fromDate, toDate);
                var reportData = await sender.Send(query);

                var excelBytes = await excelService.GenerateExcelReportAsync(reportData);

                var fileName = $"BaoCaoThongKeBenhNhan_{reportData.FromDate:yyyyMMdd}_{reportData.ToDate:yyyyMMdd}.xlsx";

                return Results.File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            })
            .RequireAuthorization()
            .WithName("ExportPatientStatisticsReport")
            .WithSummary("Export patient statistics report to Excel")
            .WithDescription("Export patient statistics report as Excel file with age groups and location analysis")
            .Produces<FileResult>()
            .ProducesProblem(400)
            .ProducesProblem(500);
        }
    }
}