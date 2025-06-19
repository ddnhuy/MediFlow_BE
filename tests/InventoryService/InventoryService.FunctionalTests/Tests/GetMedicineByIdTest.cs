using FluentAssertions;
using Inventory.Application.DTOs;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class GetMedicineByIdTests : BaseFunctionalTest
    {
        public GetMedicineByIdTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetMedicineById_WithValidId_ReturnsOk()
        {
            // Arrange
            var medicineId = 1; 

            // Act
            var response = await _client.GetAsync($"/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetMedicineById_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var medicineId = 1; 
            // Act
            var response = await _client.GetAsync($"/medicines/{medicineId}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMedicineById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var medicineId = 999; // ID that doesn't exist

            // Act
            var response = await _client.GetAsync($"/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
