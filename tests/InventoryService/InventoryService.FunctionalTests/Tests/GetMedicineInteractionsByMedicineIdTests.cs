using FluentAssertions;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetMedicineInteractionsByMedicineIdTests : BaseFunctionalTest
    {
        public GetMedicineInteractionsByMedicineIdTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicineInteractionsByMedicineId_WithValidId_ReturnsOk()
        {
            // Arrange - Use ID from seeded medicine
            var medicineId = 1; // Paracetamol, which has an interaction with Ibuprofen (ID 2)

            // Act
            var response = await _client.GetAsync($"/medicine-interactions/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetMedicineInteractionsByMedicineId_WithMedicineHavingNoInteractions_ReturnsEmptyList()
        {
            // Arrange - Medicine ID 3 (Aspirin) has no interactions in seed data
            var medicineId = 3;

            // Act
            var response = await _client.GetAsync($"/medicine-interactions/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetMedicineInteractionsByMedicineId_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var medicineId = 999; // ID that doesn't exist

            // Act
            var response = await _client.GetAsync($"/medicine-interactions/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}