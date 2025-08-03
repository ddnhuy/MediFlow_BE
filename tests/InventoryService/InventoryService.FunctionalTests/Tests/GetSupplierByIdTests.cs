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
    public class GetSupplierByIdTests : BaseFunctionalTest
    {
        public GetSupplierByIdTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetSupplierById_WithValidId_ReturnsOk()
        {
            // Arrange
            var supplierId = 1; // Assuming this ID exists in test data

            // Act
            var response = await _client.GetAsync($"/suppliers/{supplierId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetSupplierByIdResponse>();
            result.Should().NotBeNull();
            result!.Supplier.Should().NotBeNull();
            result.Supplier.Id.Should().Be(supplierId);
            result.Supplier.SupplierCode.Should().NotBeNullOrEmpty();
            result.Supplier.SupplierName.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetSupplierById_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var supplierId = 1;

            // Act
            var response = await _client.GetAsync($"/suppliers/{supplierId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetSupplierById_NotFoundData_ReturnsBadRequest()
        {
            // Arrange
            var supplierId = 999999; // Invalid negative ID

            // Act
            var response = await _client.GetAsync($"/suppliers/{supplierId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}