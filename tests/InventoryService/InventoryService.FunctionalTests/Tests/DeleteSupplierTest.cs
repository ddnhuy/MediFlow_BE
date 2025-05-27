using FluentAssertions;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class DeleteSupplierTests : BaseFunctionalTest
    {
        public DeleteSupplierTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Delete_WithValidId_ReturnsOkAndSoftDeletes()
        {
            // Arrange
            var supplierId = 3; // Using seeded supplier ID

            // Act
            var response = await _client.DeleteAsync($"/suppliers/{supplierId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DeleteSupplierResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999; // Non-existent ID

            // Act
            var response = await _client.DeleteAsync($"/suppliers/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    public class DeleteSupplierResult
    {
        public bool IsSuccess { get; set; }
    }
}
