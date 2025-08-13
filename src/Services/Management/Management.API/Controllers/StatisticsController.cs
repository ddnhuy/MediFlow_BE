using Management.API.Statistics.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = $"{BuildingBlocks.Strings.Roles.ADMIN},{BuildingBlocks.Strings.Roles.HEAD_OF_DEPARTMENT}")]
    public class StatisticsController(ISender sender) : ControllerBase
    {
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverviewAsync()
        {
            var getVaccineTrafficTask = sender.Send(new GetVaccineTrafficQuery());

            var getTodayPatientCountTask = sender.Send(new GetTodayPatientCountQuery());
            var getTodayInjectionCountTask = sender.Send(new GetTodayInjectionCountQuery());
            var getTodayRevenueTask = sender.Send(new GetTodayRevenueQuery());
            var getProcessedReceptionsCountTask = sender.Send(new GetProcessedReceptionsCountQuery());

            var getTotalPatientsByYearMonthTask = sender.Send(new GetTotalPatientsByYearMonthQuery());
            var getTotalRevenueByYearMonthTask = sender.Send(new GetTotalRevenueByYearMonthQuery());

            var vaccineTraffic = await getVaccineTrafficTask;
            var todayPatientCount = await getTodayPatientCountTask;
            var todayInjectionCount = await getTodayInjectionCountTask;
            var todayRevenue = await getTodayRevenueTask;
            var processedReceptionsCount = await getProcessedReceptionsCountTask;

            var totalPatientsByYearMonth = await getTotalPatientsByYearMonthTask;
            var totalRevenueByYearMonth = await getTotalRevenueByYearMonthTask;

            return Ok(new
            {
                VaccineTraffic = vaccineTraffic.VaccineTraffic,
                TodayPatientCount = todayPatientCount,
                TodayInjectionCount = todayInjectionCount,
                TodayRevenue = todayRevenue,
                ProcessedReceptionsCount = processedReceptionsCount,
                TotalPatientsByYearMonth = totalPatientsByYearMonth.YearlyPatientList,
                TotalRevenueByYearMonth = totalRevenueByYearMonth.YearlyRevenueList
            });
        }
    }
}
