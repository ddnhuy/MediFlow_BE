// tests/InventoryService/InventoryService.FunctionalTests/Tests/ExportMedicineRevenueReportTests.cs
using FluentAssertions;
using Grpc.Core;
using HumanResource.Grpc;
using InventoryService.FunctionalTests.Abstractions;
using NSubstitute;
using System.Net;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class ExportMedicineRevenueReportTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;
        public ExportMedicineRevenueReportTests(FunctionalTestWebAppFactory factory) : base(factory) {
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
        public async Task ExportMedicineRevenueReport_WithDefaultDateRange_ReturnsExcelFile()
        {
            // Act
            var response = await _client.GetAsync("/medicine-revenue/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var contentDisposition = response.Content.Headers.ContentDisposition;
            contentDisposition.Should().NotBeNull();
            contentDisposition!.DispositionType.Should().Be("attachment");
            contentDisposition.FileName.Should().StartWith("BaoCaoDoanhSoThuoc_");
            contentDisposition.FileName.Should().EndWith(".xlsx");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty("Excel file should not be empty");
            content.Length.Should().BeGreaterThan(1000, "Excel file should have reasonable size");
        }

        [Fact]
        public async Task ExportMedicineRevenueReport_WithValidDateRange_ReturnsExcelFile()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/medicine-revenue/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var contentDisposition = response.Content.Headers.ContentDisposition;
            contentDisposition?.FileName.Should().Contain(fromDate.ToString("yyyyMMdd"));
            contentDisposition?.FileName.Should().Contain(toDate.ToString("yyyyMMdd"));
        }

        [Fact]
        public async Task ExportMedicineRevenueReport_WithMedicineCategory_ReturnsExcelFile()
        {
            // Arrange
            var medicineCategory = "Vaccine";

            // Act
            var response = await _client.GetAsync($"/medicine-revenue/export?medicineCategory={medicineCategory}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ExportMedicineRevenueReport_VerifyExcelFileStructure()
        {
            // Act
            var response = await _client.GetAsync("/medicine-revenue/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();

            // Verify it's a valid Excel file by checking Excel file signature
            // Excel files start with PK (ZIP signature) since they are ZIP archives
            content[0].Should().Be(0x50); // 'P'
            content[1].Should().Be(0x4B); // 'K'
        }

        [Fact]
        public async Task ExportMedicineRevenueReport_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)); // Invalid: toDate before fromDate

            // Act
            var response = await _client.GetAsync($"/medicine-revenue/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ExportMedicineRevenueReport_VerifyResponseTime()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var response = await _client.GetAsync("/medicine-revenue/export");
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000, "Excel export should complete within 10 seconds");
        }

        [Fact]
        public async Task ExportMedicineRevenueReport_WithLargeDateRange_ReturnsExcelFile()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-365)); // 1 year ago
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/medicine-revenue/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }
    }
}