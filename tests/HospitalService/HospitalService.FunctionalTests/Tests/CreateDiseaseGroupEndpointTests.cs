using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class CreateDiseaseGroupEndpointTests : BaseFunctionalTest
    {
        public CreateDiseaseGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreateDiseaseGroup_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new { GroupName = "Test Disease Group", Description = "Test Description", ServiceIds = new List<int> { 1, 2 } };

            var response = await _client.PostAsJsonAsync("/disease-groups", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateDiseaseGroup_InvalidRequest_Returns400()
        {
            var request = new { GroupName = "", Description = "Test Description", ServiceIds = new List<int> { 1 } };

            var response = await _client.PostAsJsonAsync("/disease-groups", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateDiseaseGroup_ValidRequest_Returns201()
        {
            var request = new { GroupName = "Test Disease Group", Description = "Test Description", ServiceIds = new List<int> { 1, 2 } };

            var response = await _client.PostAsJsonAsync("/disease-groups", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }
    }
}
