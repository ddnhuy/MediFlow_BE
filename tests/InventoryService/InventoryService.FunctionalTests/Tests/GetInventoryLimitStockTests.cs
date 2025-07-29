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

        [Theory]
        [InlineData("aspirin", "Medicine name search")]
        [InlineData("ASP001", "Medicine code search")]
        [InlineData("ibuprofen", "Partial medicine name search")]
        [InlineData("IBU", "Partial medicine code search")]
        [InlineData("", "Empty search keyword")]
        [InlineData("   ", "Whitespace search keyword")]
        [InlineData("nonexistentmedicine", "Non-existent medicine search")]
        public async Task GetInventoryLimitStock_WithSearchKeyword_ReturnsFilteredResults(string searchKeyword, string testDescription)
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };
            var searchParam = string.IsNullOrWhiteSpace(searchKeyword) ? "" : $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks?pageIndex={request.PageIndex}&pageSize={request.PageSize}{searchParam}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK, testDescription);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GetInventoryLimitStockResponse>();
                result.Should().NotBeNull(testDescription);
                result!.InventoryLimitStocks.Should().NotBeNull(testDescription);

                // If search keyword is provided and not empty/whitespace, verify filtering
                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    var searchTerm = searchKeyword.Trim().ToLower();
                    var hasMatchingResults = result.InventoryLimitStocks.Data.Any(item =>
                        (item.MedicineName?.ToLower().Contains(searchTerm) == true) ||
                        (item.MedicineCode?.ToLower().Contains(searchTerm) == true));

                    // For non-existent searches, we might get empty results, which is valid
                    // For existing searches, we should have matching results
                    if (searchTerm != "nonexistentmedicine")
                    {
                        // If there are results, at least one should match the search criteria
                        if (result.InventoryLimitStocks.Data.Any())
                        {
                            hasMatchingResults.Should().BeTrue($"Search results should contain items matching '{searchTerm}'");
                        }
                    }
                }
            }
        }
    }
}