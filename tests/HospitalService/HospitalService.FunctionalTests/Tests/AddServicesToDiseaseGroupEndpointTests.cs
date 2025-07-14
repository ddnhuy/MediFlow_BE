using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class AddServicesToDiseaseGroupEndpointTests : BaseFunctionalTest
    {
        public AddServicesToDiseaseGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task AddServicesToDiseaseGroup_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { ServiceIds = new[] { 1, 2, 3 } };

            var response = await _client.PostAsJsonAsync("/disease-groups/1/services", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddServicesToDiseaseGroup_InvalidRequest_Returns400()
        {
            var request = new { ServiceIds = new int[] { } };

            var response = await _client.PostAsJsonAsync("/disease-groups/1/services", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddServicesToDiseaseGroup_ValidRequest_Returns200()
        {
            var request = new { ServiceIds = new[] { 1, 2 } };

            var response = await _client.PostAsJsonAsync("/disease-groups/1/services", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }

        [Fact]
        public async Task AddServicesToDiseaseGroup_InvalidId_Return404()
        {
            var request = new { ServiceIds = new[] { 1, 2 } };

            var response = await _client.PostAsJsonAsync("/disease-groups/9999/services", request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
