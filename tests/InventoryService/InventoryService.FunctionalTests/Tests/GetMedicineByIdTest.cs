using FluentAssertions;
using Inventory.API.Endpoints;
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
            var result = await response.Content.ReadFromJsonAsync<GetMedicineByIdResponse>();
            result!.Medicine.UnitPrice.Should().Be(625000m); // From seed data
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
        public async Task GetMedicineById_WithValidIdAndNoPrice_ReturnsOkWithNullPrice()
        {
            // Arrange
            var medicineId = 3; // This medicine might not have a price in test data

            // Act
            var response = await _client.GetAsync($"/medicines/{medicineId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineByIdResponse>();
            result.Should().NotBeNull();
            result!.Medicine.Should().NotBeNull();
            result.Medicine.Id.Should().Be(medicineId);

            // Verify UnitPrice field is present (can be null)
            result.Medicine.UnitPrice.Should().BeNull();
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
