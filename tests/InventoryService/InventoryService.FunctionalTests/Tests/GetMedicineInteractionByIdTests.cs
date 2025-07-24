using FluentAssertions;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetMedicineInteractionByIdTests : BaseFunctionalTest
    {
        public GetMedicineInteractionByIdTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicineInteractionById_WithValidId_ReturnsOk()
        {
            // Arrange - Use ID from seeded interaction
            var interactionId = 1; // Make sure this exists in your seed data

            // Act
            var response = await _client.GetAsync($"/medicine-interactions/{interactionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetMedicineInteractionById_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var interactionId = 1;

            // Act
            var response = await _client.GetAsync($"/medicine-interactions/{interactionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineInteractionById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var interactionId = 9999; // Non-existent ID

            // Act
            var response = await _client.GetAsync($"/medicine-interactions/{interactionId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}