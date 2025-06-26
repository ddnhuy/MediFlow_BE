using BuildingBlocks.Pagination;
using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryService.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetVaccinesRequireTestEndpointTests : BaseFunctionalTest
    {
        public GetVaccinesRequireTestEndpointTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetVaccinesRequireTest_WithValidPagination_ReturnsOk()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };
            var search = "test";
            // Act
            var response = await _client.GetAsync($"/medicines/vaccines-require-test?pageIndex={request.PageIndex}&pageSize={request.PageSize}&search={search}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetVaccinesRequireTest_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines/vaccines-require-test?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetVaccinesRequireTest_WithInvalidPagination_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = -1, PageSize = 0 }; // Invalid pagination

            // Act
            var response = await _client.GetAsync($"/medicines/vaccines-require-test?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
