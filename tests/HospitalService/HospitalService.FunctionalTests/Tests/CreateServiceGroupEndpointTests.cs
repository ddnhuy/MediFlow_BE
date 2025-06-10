using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class CreateServiceGroupEndpointTests : BaseFunctionalTest
    {
        public CreateServiceGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreateServiceGroup_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { GroupName = "Test Group", ServiceIds = new List<int> { 1, 2 } };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceGroup_InvalidRequest_Returns400()
        {
            // Arrange
            var request = new { GroupName = "", ServiceIds = new List<int> { 1 } };
            // Act
            var response = await _client.PostAsJsonAsync("/service-groups", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceGroup_ValidRequest_Returns200()
        {
            // Arrange
            var request = new { GroupName = "Test Group", ServiceIds = new List<int> { 1, 2 } };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }
    }
}