using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class UpdateDiseaseGroupEndpointTests : BaseFunctionalTest
    {
        public UpdateDiseaseGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task UpdateDiseaseGroup_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            var request = new
            {
                GroupName = "Updated Disease Group",
                Description = "Updated Description"
            };

            var response = await _client.PutAsJsonAsync("/disease-groups/1", request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDiseaseGroup_InvalidRequest_Returns400()
        {
            var request = new
            {
                GroupName = "", // Empty group name invalid
                Description = "Test Description"
            };

            var response = await _client.PutAsJsonAsync("/disease-groups/1", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateDiseaseGroup_ValidRequest_Returns200()
        {
            var request = new
            {
                GroupName = "Updated Disease Group",
                Description = "Updated Description"
            };

            var response = await _client.PutAsJsonAsync("/disease-groups/1", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
        }

        [Fact]
        public async Task UpdateDiseaseGroup_NonExistentId_Returns404()
        {
            var request = new
            {
                GroupName = "Updated Disease Group",
                Description = "Updated Description"
            };

            var response = await _client.PutAsJsonAsync("/disease-groups/999", request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
