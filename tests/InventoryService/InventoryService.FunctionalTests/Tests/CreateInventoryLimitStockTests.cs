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
    public class CreateInventoryLimitStockTests : BaseFunctionalTest
    {
        public CreateInventoryLimitStockTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task CreateInventoryLimitStock_WithValidData_ReturnsOk()
        {
            // Arrange: Use a valid, non-duplicate MedicineId from your seeded data
            var command = new CreateInventoryLimitStockCommand(
                MedicineId: 2, // Use a valid, non-duplicate ID
                MinimalStockThreshold: 10
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory-limit-stocks", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CreateInventoryLimitStockResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CreateInventoryLimitStock_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateInventoryLimitStockCommand(
                MedicineId: 2,
                MinimalStockThreshold: 10
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory-limit-stocks", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateInventoryLimitStock_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange: Invalid MedicineId and negative threshold
            var command = new CreateInventoryLimitStockCommand(
                MedicineId: 0,
                MinimalStockThreshold: -5
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory-limit-stocks", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateInventoryLimitStock_WithDuplicateMedicineId_ReturnsBadRequest()
        {
            // Arrange: First, create a valid record
            var command = new CreateInventoryLimitStockCommand(
                MedicineId: 4, // Use a unique ID for this test
                MinimalStockThreshold: 10
            );
            var firstResponse = await _client.PostAsJsonAsync("/inventory-limit-stocks", command);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Act: Try to create again with the same MedicineId
            var duplicateResponse = await _client.PostAsJsonAsync("/inventory-limit-stocks", command);

            // Assert
            duplicateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await duplicateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }
    }
}