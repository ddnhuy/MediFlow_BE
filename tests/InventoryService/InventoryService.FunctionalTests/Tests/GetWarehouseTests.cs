using BuildingBlocks.Pagination;
using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetWarehousesTests : BaseFunctionalTest
    {
        public GetWarehousesTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetWarehouses_WithValidPagination_ReturnsOk()
        {
            // Arrange - ensure warehouses are seeded in DatabaseSeeder.cs
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/warehouses?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetWarehousesResponse>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetWarehouses_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };
            // Act
            var response = await _client.GetAsync($"/warehouses?pageIndex={request.PageIndex}&pageSize={request.PageSize}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetWarehouses_WithInvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = -1, PageSize = 0 }; // Invalid pagination
            // Act
            var response = await _client.GetAsync($"/warehouses?pageIndex={request.PageIndex}&pageSize={request.PageSize}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}