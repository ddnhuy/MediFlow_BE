using FluentAssertions;
using Inventory.Application.Medicines.Commands.DeleteMedicine;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class DeleteMedicineTests : BaseFunctionalTest
    {
        public DeleteMedicineTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Delete_WithValidId_ReturnsOkAndSoftDeletes()
        {
            // Arrange
            var medicineId = 3;

            // Act
            var response = await _client.DeleteAsync($"/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DeleteMedicineResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var medicineId = 3;

            var response = await _client.DeleteAsync($"/medicines/{medicineId}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999; // Non-existent ID

            // Act
            var response = await _client.DeleteAsync($"/medicines/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}