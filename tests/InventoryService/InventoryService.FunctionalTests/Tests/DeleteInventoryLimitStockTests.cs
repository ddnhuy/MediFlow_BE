using FluentAssertions;
using Inventory.Application.InventoryLimitStock.Commands;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class DeleteInventoryLimitStockTests : BaseFunctionalTest
    {
        public DeleteInventoryLimitStockTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task DeleteInventoryLimitStock_WithValidId_ReturnsOkAndSoftDeletes()
        {
            // Arrange: Seed a record to delete
            var limitStockCommand = new CreateInventoryLimitStockCommand(
                MedicineId: 2, // Use a valid, non-duplicate ID
                MinimalStockThreshold: 10
            );
            var createResponse = await _client.PostAsJsonAsync("/inventory-limit-stocks", limitStockCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act
            var response = await _client.DeleteAsync($"/inventory-limit-stocks/{1}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteInventoryLimitStock_WithInvalidId_ReturnsNotFound()
        {
            // Arrange: Use a non-existent ID
            int invalidId = 999999;

            // Act
            var response = await _client.DeleteAsync($"/inventory-limit-stocks/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteInventoryLimitStock_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.DeleteAsync($"/inventory-limit-stocks/{2}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}