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
    public class GetInventoryLimitStockTests : BaseFunctionalTest
    {
        public GetInventoryLimitStockTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetInventoryLimitStock_WithValidPagination_ReturnsOk()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetInventoryLimitStock_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetInventoryLimitStock_WithInvalidPagination_ReturnsBadRequest()
        {
            // Arrange - Invalid pagination parameters
            var invalidRequest = new PaginationRequest { PageIndex = -1, PageSize = 0 };

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks?pageIndex={invalidRequest.PageIndex}&pageSize={invalidRequest.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetInventoryLimitStock_WithLargePageSize_ReturnsBadRequest()
        {
            // Arrange - Page size exceeds maximum allowed
            var request = new PaginationRequest { PageIndex = 1, PageSize = 1001 };

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetInventoryLimitStock_WithZeroPageIndex_ReturnsBadRequest()
        {
            // Arrange - Page index cannot be zero
            var request = new PaginationRequest { PageIndex = 0, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}