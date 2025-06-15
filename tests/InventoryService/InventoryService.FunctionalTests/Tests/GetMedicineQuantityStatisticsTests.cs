using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetMedicineQuantityStatisticsTests : BaseFunctionalTest
    {
        public GetMedicineQuantityStatisticsTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicineQuantityStatistics_WhenAuthorized_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/medicine-quantity-statistics?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineQuantityStatisticsResponse>();
            result.Should().NotBeNull();
            result!.Statistics.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMedicineQuantityStatistics_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/medicine-quantity-statistics?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineQuantityStatistics_WithInvalidPagination_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/medicine-quantity-statistics?pageIndex=-1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}