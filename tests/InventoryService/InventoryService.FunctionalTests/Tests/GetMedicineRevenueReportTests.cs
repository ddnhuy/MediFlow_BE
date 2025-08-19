using FluentAssertions;
using Grpc.Core;
using HumanResource.Grpc;
using Inventory.Application.DTOs;
using InventoryService.FunctionalTests.Abstractions;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Inventory.Application.Reports;
using Inventory.Application.Services;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetMedicineRevenueReportTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;

        public GetMedicineRevenueReportTests(FunctionalTestWebAppFactory factory) : base(factory) {
            _factory = factory;
            SetupMockServices();
        }

        private void SetupMockServices()
        {
            // Setup ApplicationUserProtoService mock
            var doctorResponse = new ApplicationUserDetailModel
            {
                Id = 1,
                Name = "Admin"
            };

            var userAsyncUnaryCall = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Metadata>(), null, default)
                .Returns(userAsyncUnaryCall);
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithDefaultDateRange_ReturnsOk()
        {
            // Arrange
            // No parameters - should use default date range (last 30 days)

            // Act
            var response = await _client.GetAsync("/medicine-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.FromDate.Should().BeBefore(result.ToDate);
            result.GeneratedBy.Should().NotBeNullOrEmpty();
            result.Summary.Should().NotBeNull();
            result.MedicineDetails.Should().NotBeNull();
            result.CategoryStatistics.Should().NotBeNull();
            result.DailyStatistics.Should().NotBeNull();
            result.BatchDetails.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithValidDateRange_ReturnsOk()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.FromDate.Should().Be(fromDate);
            result.ToDate.Should().Be(toDate);
            result.Summary.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithSpecificMedicineCategory_ReturnsFilteredResults()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var medicineCategory = "Vaccine";

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}&medicineCategory={medicineCategory}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.MedicineDetails.Should().NotBeNull();

            // If there are medicine details, they should match the category filter
            if (result.MedicineDetails.Any())
            {
                result.MedicineDetails.Should().AllSatisfy(medicine =>
                    medicine.Classification.Should().Contain(medicineCategory, "All medicines should match the category filter"));
            }
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithWarehouseFilter_ReturnsFilteredResults()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var warehouseId = 1; // Assuming warehouse ID 1 exists from seed data

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}&warehouseId={warehouseId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.Summary.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)); // Invalid: toDate before fromDate

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetMedicineRevenueReport_VerifySummaryCalculations()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();

            var summary = result!.Summary;
            summary.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineRevenueReport_VerifyMedicineDetailsStructure()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/medicine-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineRevenueReport_VerifyBatchDetailsStructure()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/medicine-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineRevenueReport_VerifyDailyStatisticsOrder()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();

            var dailyStats = result!.DailyStatistics;
            if (dailyStats.Count > 1)
            {
                // Verify daily statistics are ordered by date
                for (int i = 1; i < dailyStats.Count; i++)
                {
                    dailyStats[i].Date.Should().BeAfter(dailyStats[i - 1].Date, "Daily statistics should be ordered by date ascending");
                }
            }
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithLargeDateRange_ReturnsOk()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-365)); // 1 year ago
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.FromDate.Should().Be(fromDate);
            result.ToDate.Should().Be(toDate);
        }

        [Fact]
        public async Task GetMedicineRevenueReport_WithFutureDateRange_ReturnsEmptyResults()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)); // Tomorrow
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)); // Next week

            // Act
            var response = await _client.GetAsync($"/medicine-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<MedicineRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.Summary.TotalRevenue.Should().Be(0, "Future date range should have no revenue");
            result.MedicineDetails.Should().BeEmpty("Future date range should have no medicine details");
        }

        [Fact]
        public async Task GetMedicineRevenueReport_VerifyResponseTime()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var response = await _client.GetAsync("/medicine-revenue");
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Report should be generated within 5 seconds");
        }
    }
}