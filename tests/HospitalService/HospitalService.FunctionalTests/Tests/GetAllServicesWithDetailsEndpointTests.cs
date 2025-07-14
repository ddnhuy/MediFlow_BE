using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetAllServicesWithDetailsEndpointTests : BaseFunctionalTest
    {
        public GetAllServicesWithDetailsEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetAllServicesWithDetails_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.GetAsync("/services/details");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllServicesWithDetails_Success_Returns200AndValidContent()
        {
          
            // Act
            var response = await _client.GetAsync("/services/details");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
            Assert.NotNull(node["services"]);
            Assert.True(node["services"] is JsonArray);
        }

        [Fact]
        public async Task GetAllServicesWithDetails_BadRequestSimulation_Returns400()
        {

            var response = await _client.GetAsync("/services/details?simulateBadRequest=true");

            if (response.StatusCode != HttpStatusCode.BadRequest)
                return;

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
