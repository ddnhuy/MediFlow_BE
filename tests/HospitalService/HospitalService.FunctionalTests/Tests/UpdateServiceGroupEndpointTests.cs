using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class UpdateServiceGroupEndpointTests : BaseFunctionalTest
    {
        public UpdateServiceGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task UpdateServiceGroup_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { GroupName = "Updated Group" };

            // Act
            var response = await _client.PutAsJsonAsync("/servicegroups/1", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateServiceGroup_InvalidRequest_Returns400()
        {
            // Arrange
            var request = new { GroupName = "" }; // Empty group name invalid

            // Act
            var response = await _client.PutAsJsonAsync("/servicegroups/1", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateServiceGroup_ValidRequest_Returns200()
        {
            // Arrange
            var request = new { GroupName = "Updated Group" };

            // Act
            var response = await _client.PutAsJsonAsync("/servicegroups/1", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }
    }
}