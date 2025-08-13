using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Infrastructure.Data;

namespace VaccinationReception.API.Services
{
    public class VaccinationReceptionService(
        ILogger<VaccinationReceptionService> logger,
        ApplicationDbContext dbContext) : VaccinationReceptionProtoService.VaccinationReceptionProtoServiceBase
    {
        public override async Task<ProcessedReceptionsCountResponse> GetProcessedReceptionsCount(EmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetProcessedReceptionsCount called with request: {Request}", request);

            var todayProcessedReceptionCount = await dbContext.Receptions
                .AsNoTracking()
                .Where(x => x.ReceptionDate.Date == DateTime.UtcNow.Date && x.IsVaccinationTodayConfirmed)
                .LongCountAsync(context.CancellationToken);

            logger.LogInformation("Total processed receptions count for today: {Count}", todayProcessedReceptionCount);

            return new ProcessedReceptionsCountResponse
            {
                Count = todayProcessedReceptionCount
            };
        }

        public override async Task<TodayInjectionCountResponse> GetTodayInjectionCount(EmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetTodayInjectionCount called with request: {Request}", request);

            var todayInjectionCount = await dbContext.Vaccinations
                .AsNoTracking()
                .Where(x => x.VaccinationDate != null && x.VaccinationDate.Value.Date == DateTime.UtcNow.Date && x.IsConfirmed)
                .LongCountAsync(context.CancellationToken);

            logger.LogInformation("Total injections count for today: {Count}", todayInjectionCount);

            return new TodayInjectionCountResponse
            {
                Count = todayInjectionCount
            };
        }

        public override async Task<TodayPatientCountResponse> GetTodayPatientCount(EmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetTodayPatientCount called with request: {Request}", request);

            var todayPatientCount = await dbContext.Receptions
                .AsNoTracking()
                .Where(x => x.ReceptionDate.Date == DateTime.UtcNow.Date && x.IsVaccinationTodayConfirmed)
                .Select(x => x.PatientId)
                .Distinct()
                .LongCountAsync(context.CancellationToken);

            logger.LogInformation("Total patients count for today: {Count}", todayPatientCount);

            return new TodayPatientCountResponse
            {
                Count = todayPatientCount
            };
        }

        public override async Task<TodayRevenueResponse> GetTodayRevenue(EmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetTodayRevenue called with request: {Request}", request);

            var todayRevenue = await dbContext.Payments
                .AsNoTracking()
                .Where(x => x.LastUpdatedAt.Date == DateTime.UtcNow.Date && x.Status == PaymentStatus.Completed)
                .SumAsync(x => x.TotalAmount);

            logger.LogInformation("Total revenue for today: {Revenue}", todayRevenue);

            return new TodayRevenueResponse
            {
                Amount = Convert.ToDouble(todayRevenue),
                Currency = "VND" // Assuming VND, adjust as necessary
            };
        }

        public override async Task<TotalPatientsByYearMonthResponse> GetTotalPatientsByYearMonth(EmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetTotalPatientsByYearMonth called with request: {Request}", request);

            var totalPatientsByYearMonth = await dbContext.Receptions
                .AsNoTracking()
                .Where(x => x.IsVaccinationTodayConfirmed)
                .GroupBy(x => new { x.ReceptionDate.Year, x.ReceptionDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Select(x => x.PatientId).Distinct().Count()
                })
                .ToListAsync(context.CancellationToken);

            var culture = new CultureInfo("en-US");
            var totalPatientsByYear = totalPatientsByYearMonth
                .GroupBy(x => x.Year)
                .Select(g =>
                {
                    var monthList = g.Select(m => new MonthlyPatientData
                    {
                        Month = culture.DateTimeFormat.GetAbbreviatedMonthName(m.Month),
                        TotalPatients = m.Count,
                    }).ToList();

                    return new YearlyPatientData
                    {
                        Year = g.Key,
                        Months =
                        {
                            monthList
                        }
                    };
                })
                .ToList();

            logger.LogInformation("Total patients by year and month: {TotalPatientsByYear}", totalPatientsByYear);

            return new TotalPatientsByYearMonthResponse
            {
                Data = { totalPatientsByYear }
            };
        }

        public override async Task<TotalRevenueByYearMonthResponse> GetTotalRevenueByYearMonth(EmptyRequest request, ServerCallContext context)
        {
            logger.LogInformation("GetTotalRevenueByYearMonth called with request: {Request}", request);

            var totalRevenueByYearMonth = await dbContext.Payments
                .AsNoTracking()
                .Where(x => x.Status == PaymentStatus.Completed)
                .GroupBy(x => new { x.LastUpdatedAt.Year, x.LastUpdatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync(context.CancellationToken);

            var culture = new CultureInfo("en-US");
            var totalRevenueByYear = totalRevenueByYearMonth
                .GroupBy(x => x.Year)
                .Select(g =>
                {
                    var monthList = g.Select(m => new MonthlyRevenueData
                    {
                        Month = culture.DateTimeFormat.GetAbbreviatedMonthName(m.Month),
                        TotalRevenue = Convert.ToDouble(m.TotalRevenue),
                        Currency = "VND" // Assuming VND, adjust as necessary
                    }).ToList();
                    return new YearlyRevenueData
                    {
                        Year = g.Key,
                        Months =
                        {
                            monthList
                        }
                    };
                })
                .ToList();

            logger.LogInformation("Total revenue by year and month: {TotalRevenueByYear}", totalRevenueByYear);

            return new TotalRevenueByYearMonthResponse
            {
                Data = { totalRevenueByYear }
            };
        }
    }
}
