using FluentAssertions;
using Inventory.Application.DTOs;
using Inventory.Application.InventoryLimitStock.Commands;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetInventoryLimitStockByIdTests : BaseFunctionalTest
    {
        public GetInventoryLimitStockByIdTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetInventoryLimitStockById_WithValidId_ReturnsOk()
        {
            // Seed data 
            var command = new CreateInventoryLimitStockCommand
            (
                MedicineId : 5, // Use a valid MedicineId from your seed data
                MinimalStockThreshold: 10
            );

            var createResponse = await _client.PostAsJsonAsync("/inventory-limit-stocks", command);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Arrange: Use a valid Id from your seed data
            int validId = 1;

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks/{validId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetInventoryLimitStockByIdResponse>();
            result.Should().NotBeNull();
            result!.InventoryLimitStock.Should().NotBeNull();
            result.InventoryLimitStock.Id.Should().Be(validId);
        }

        [Fact]
        public async Task GetInventoryLimitStockById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange: Use an ID that does not exist
            int invalidId = 999999;

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetInventoryLimitStockById_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange: Use a valid Id from your seed data
            int validId = 1;
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/inventory-limit-stocks/{validId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    // Response DTO for get by id endpoint
    public class GetInventoryLimitStockByIdResponse
    {
        public InventoryLimitStockDTO InventoryLimitStock { get; set; }
    }
}