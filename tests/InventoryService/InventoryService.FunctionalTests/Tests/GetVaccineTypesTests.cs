using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryService.FunctionalTests.Tests
{
    public class GetVaccineTypesTests : BaseFunctionalTest
    {
        public GetVaccineTypesTests(FunctionalTestWebAppFactory factory ) : base(factory)
        {
            
        }

        [Fact]
        public async Task GetVaccineTypes_WhenAuthorized_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/vaccine-types");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GetVaccineTypesResponse>();
            result.Should().NotBeNull();
            result.VaccineTypes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetVaccineTypes_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/vaccine-types");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
