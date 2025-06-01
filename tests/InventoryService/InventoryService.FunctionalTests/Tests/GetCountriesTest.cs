using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class GetCountriesTests : BaseFunctionalTest
    {
        public GetCountriesTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetCountries_WhenAuthorized_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/countries");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetCountriesResponse>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCountries_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/countries");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetCountries_WithIncorrectRoute_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/country"); // Incorrect route

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
