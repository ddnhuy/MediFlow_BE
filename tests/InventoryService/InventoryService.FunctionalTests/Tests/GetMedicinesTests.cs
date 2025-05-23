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
    public class GetMedicinesTests : BaseFunctionalTest
    {
        public GetMedicinesTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicines_WithValidPagination_ReturnsOk()
        {
            // Arrange
            var request = new PaginationRequest { PageIndex = 1, PageSize = 10 };

            // Act
            var response = await _client.GetAsync($"/medicines?pageIndex={request.PageIndex}&pageSize={request.PageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicinesResponse>();
            result.Should().NotBeNull();
            result!.Medicines.Data.Should().NotBeEmpty();
        }
    }
}