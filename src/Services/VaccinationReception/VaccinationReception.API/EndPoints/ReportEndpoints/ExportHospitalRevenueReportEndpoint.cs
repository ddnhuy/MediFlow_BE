using VaccinationReception.Application.Reports;
using VaccinationReception.Application.Services.ExcelServices;

namespace VaccinationReception.API.EndPoints.Reports
{
    public class ExportHospitalRevenueReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/hospital-revenue/export", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                ISender sender,
                [FromServices]
                IHospitalRevenueExcelService excelService) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var query = new GetHospitalRevenueReportQuery(fromDate, toDate);
                var reportData = await sender.Send(query);

                var excelBytes = await excelService.GenerateExcelReportAsync(reportData);

                var fileName = $"BaoCaoDoanhThuBenhVien_{reportData.FromDate:yyyyMMdd}_{reportData.ToDate:yyyyMMdd}.xlsx";

                return Results.File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            })
            .RequireAuthorization()
            .WithName("ExportHospitalRevenueReport")
            .WithSummary("Export hospital revenue report to Excel")
            .WithDescription("Export hospital revenue report as Excel file with summary and daily details")
            .Produces<FileResult>()
            .ProducesProblem(400)
            .ProducesProblem(500);
        }
    }
}