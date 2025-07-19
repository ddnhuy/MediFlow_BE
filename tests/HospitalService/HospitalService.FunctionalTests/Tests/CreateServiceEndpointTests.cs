using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class CreateServiceEndpointTests : BaseFunctionalTest
    {
        public CreateServiceEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreateService_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { Name = "Test Service", Description = "Test Description" };

            // Act
            var response = await _client.PostAsJsonAsync("/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateService_InvalidRequest_Returns400()
        {
            // Arrange
            var request = new { Name = "", Description = "Test Description" };

            // Act
            var response = await _client.PostAsJsonAsync("/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateService_ValidRequest_Returns200()
        {
            var request = new { ServiceCode = "Abc123", ServiceName = "Test Description", UnitPrice = 100, DepartmentId = 1, Unit = "Test", StandardValue = "Test", Quantity = 1, EquipmentUsed = "test"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }
    }
}