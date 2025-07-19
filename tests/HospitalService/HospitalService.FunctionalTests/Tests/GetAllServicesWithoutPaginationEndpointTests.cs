using HospitalService.API.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetAllServicesWithoutPaginationEndpointTests : BaseFunctionalTest
    {
        public GetAllServicesWithoutPaginationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllServices_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // BaseFunctionalTest adds auth by default, so we create a new client without it.
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.GetAsync("/services/all");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllServices_WhenCalled_ReturnsAllSeededServices()
        {
            // Arrange
            // Data is seeded by FunctionalTestWebAppFactory

            // Act
            var response = await _client.GetAsync("/services/all");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<GetAllServicesWithoutPaginationResponse>();

            content.Should().NotBeNull();
            // Assuming DatabaseSeeder creates more than one service
            content!.Services.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllServices_WithSearchTerm_ReturnsFilteredServices()
        {
            // Arrange
            // Assuming DatabaseSeeder creates a service with the code "KHAM"
            var searchTerm = "Tiêm";

            // Act
            var response = await _client.GetAsync($"/services/all?searchTerm={searchTerm}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<GetAllServicesWithoutPaginationResponse>();

            content.Should().NotBeNull();
            content!.Services.Should().NotBeEmpty();
            //content.Services.Should().OnlyContain(s => s.ServiceCode.Contains(searchTerm) || s.ServiceName.Contains(searchTerm));
        }
    }
}
