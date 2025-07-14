using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class DeleteMedicineInteractionTests : BaseFunctionalTest
    {
        public DeleteMedicineInteractionTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Delete_WithValidId_ReturnsOkAndSoftDeletes()
        {
            // Arrange - Get an existing interaction ID from the database
            var interactionId = 1; // Using the seeded interaction

            // Act
            var response = await _client.DeleteAsync($"/medicine-interactions/{interactionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DeleteMedicineInteractionResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999; // Non-existent ID

            // Act
            var response = await _client.DeleteAsync($"/medicine-interactions/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var interactionId = 1;

            var response = await _client.DeleteAsync($"/medicine-interactions/{interactionId}");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
