using System.Text.Json.Nodes;

namespace HospitalService.FunctionalTests.Tests
{
    public class AddServicesToGroupEndpointTests : BaseFunctionalTest
    {
        public AddServicesToGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task AddServicesToGroup_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { ServiceIds = new[] { 1, 2, 3 } };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/1/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddServicesToGroup_InvalidRequest_Returns400()
        {
            // Arrange
            var request = new { ServiceIds = new int[] { } }; // Empty array should be invalid

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/1/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddServicesToGroup_ValidRequest_Returns200()
        {
            // Arrange
            var request = new { ServiceIds = new[] { 1, 2} };

            // Act
            var response = await _client.PostAsJsonAsync("/service-groups/1/services", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }
    }
}
