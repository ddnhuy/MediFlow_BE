using BuildingBlocks.Pagination;
using FluentAssertions;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetSuppliersTests : BaseFunctionalTest
    {
        public GetSuppliersTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetSuppliers_WithValidPagination_ReturnsOk()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/suppliers?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetSuppliersResponse>();
            result.Should().NotBeNull();
            result!.Suppliers.Data.Should().NotBeEmpty();
        }
    }

    public class GetSuppliersResponse
    {
        public PaginatedResult<SupplierDto> Suppliers { get; set; } = default!;
    }

    public class SupplierDto
    {
        public int Id { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        // Other properties
    }
}
