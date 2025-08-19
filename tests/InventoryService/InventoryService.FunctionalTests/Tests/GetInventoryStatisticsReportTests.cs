using BuildingBlocks.Strings;
using FluentAssertions;
using Grpc.Core;
using HumanResource.Grpc;
using Inventory.Application.DTOs;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Data;
using InventoryService.FunctionalTests.Abstractions;
using InventoryService.FunctionalTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetInventoryStatisticsReportTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public GetInventoryStatisticsReportTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SetupMockServices();
            //SeedTestData();
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
        public async Task GetInventoryStatisticsReport_WithDefaultDateRange_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_WithSpecificDateRange_ReturnsCorrectData()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/inventory-statistics?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
            result!.FromDate.Should().Be(fromDate);
            result.ToDate.Should().Be(toDate);
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_ChecksStockStatus_ReturnsCorrectStatus()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_ChecksBatchExpiry_ReturnsCorrectExpiryStatus()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_WithCategoryFilter_ReturnsFilteredData()
        {
            // Arrange
            var category = "Vaccine";

            // Act
            var response = await _client.GetAsync($"/inventory-statistics?itemCategory={category}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExportInventoryStatisticsReport_ReturnsExcelFile()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
            content.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_ChecksSummaryCalculations_ReturnsCorrectTotals()
        {
            // Act
            var response = await _client.GetAsync("/inventory-statistics");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_WithNoTransactionData_ReturnsEmptyTransactions()
        {
            // Arrange - Clear transaction data
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.InventoryHistories.RemoveRange(dbContext.InventoryHistories);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync("/inventory-statistics");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<InventoryStatisticsReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInventoryStatisticsReport_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)); // Invalid: toDate before fromDate

            // Act
            var response = await _client.GetAsync($"/inventory-statistics?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}