using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetManufacturersTests : BaseFunctionalTest
    {
        public GetManufacturersTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetManufacturers_WhenAuthorized_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/manufacturers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetManufacturersResponse>();
            result.Should().NotBeNull();
            result!.Manufacturers.Should().NotBeNull();
        }

        [Fact]
        public async Task GetManufacturers_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/manufacturers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetManufacturers_WithIncorrectRoute_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/manufacturer"); // Incorrect route

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
