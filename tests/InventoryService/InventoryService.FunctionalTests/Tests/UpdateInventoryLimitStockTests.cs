using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.InventoryLimitStock.Commands;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class UpdateInventoryLimitStockTests : BaseFunctionalTest
    {
        public UpdateInventoryLimitStockTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task UpdateInventoryLimitStock_WithValidData_ReturnsOk()
        {
            // Arrange: Create a record to update
            var createCommand = new CreateInventoryLimitStockCommand(
                MedicineId: 2,
                MinimalStockThreshold: 10
            );
            var createResponse = await _client.PostAsJsonAsync("/inventory-limit-stocks", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            int inventoryLimitStockId = 1; // Replace with actual ID retrieval if needed

            var updateCommand = new UpdateInventoryLimitStockCommand(
                Id: inventoryLimitStockId,
                MedicineId: 2,
                MinimalStockThreshold: 20,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/inventory-limit-stocks/{updateCommand.Id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateInventoryLimitStockResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateInventoryLimitStock_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var updateCommand = new UpdateInventoryLimitStockCommand(
                Id: 1,
                MedicineId: 1,
                MinimalStockThreshold: 20,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/inventory-limit-stocks/{updateCommand.Id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateInventoryLimitStock_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange: Negative threshold
            var updateCommand = new UpdateInventoryLimitStockCommand(
                Id: 1,
                MedicineId: 1,
                MinimalStockThreshold: -10,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/inventory-limit-stocks/{updateCommand.Id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateInventoryLimitStock_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange: ID in route and body do not match
            var updateCommand = new UpdateInventoryLimitStockCommand(
                Id: 2,
                MedicineId: 1,
                MinimalStockThreshold: 15,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/inventory-limit-stocks/999", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateInventoryLimitStock_WithNonExistentId_ReturnsBadRequest()
        {
            // Arrange: Use a non-existent ID
            var updateCommand = new UpdateInventoryLimitStockCommand(
                Id: 99999,
                MedicineId: 1,
                MinimalStockThreshold: 15,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/inventory-limit-stocks/{updateCommand.Id}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}