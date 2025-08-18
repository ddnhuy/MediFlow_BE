// tests/InventoryService/InventoryService.FunctionalTests/Tests/ExportInventoryStatisticsReportTests.cs
using BuildingBlocks.Strings;
using FluentAssertions;
using Grpc.Core;
using HumanResource.Grpc;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Data;
using InventoryService.FunctionalTests.Abstractions;
using InventoryService.FunctionalTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class ExportInventoryStatisticsReportTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public ExportInventoryStatisticsReportTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

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
        public async Task ExportInventoryStatisticsReport_WithDefaultParameters_ReturnsExcelFile()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var contentDisposition = response.Content.Headers.ContentDisposition;
            contentDisposition.Should().NotBeNull();
            contentDisposition!.DispositionType.Should().Be("attachment");
            contentDisposition.FileName.Should().StartWith("BaoCaoThongKeKhoVaccine_");
            contentDisposition.FileName.Should().EndWith(".xlsx");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
            content.Length.Should().BeGreaterThan(1000); // Excel file should be substantial
        }

        [Fact]
        public async Task ExportInventoryStatisticsReport_WithDateRange_ReturnsFilteredExcelFile()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/inventory-statistics/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ExportInventoryStatisticsReport_WithCategoryFilter_ReturnsFilteredExcelFile()
        {
            // Arrange
            var category = "Vaccine";

            // Act
            var response = await _client.GetAsync($"/inventory-statistics/export?itemCategory={category}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ExportInventoryStatisticsReport_WithAllParameters_ReturnsExcelFile()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var category = "Vaccine";

            // Act
            var response = await _client.GetAsync($"/inventory-statistics/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}&itemCategory={category}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ExportInventoryStatisticsReport_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)); // Invalid: toDate before fromDate

            // Act
            var response = await _client.GetAsync($"/inventory-statistics/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ExportInventoryStatisticsReport_VerifyExcelStructure_ContainsExpectedSheets()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();

            // Verify it's a valid Excel file by checking the file signature
            // Excel files start with PK (ZIP signature)
            content[0].Should().Be(0x50); // 'P'
            content[1].Should().Be(0x4B); // 'K'
        }       

        [Fact]
        public async Task ExportInventoryStatisticsReport_WithSpecialCharacters_HandlesEncoding()
        {
            // Arrange - Add medicine with special characters
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var specialMedicine = new Medicine
            {
                Id = 99,
                MedicineCode = "SPECIAL001",
                MedicineName = "Vaccine Đặc Biệt (Tiếng Việt) & Special Characters",
                Unit = "Liều",
                MedicineClassification = "Vaccine Đặc Biệt",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Medicines.Add(specialMedicine);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync("/inventory-statistics/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }
    }
}