using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetAllDiseaseGroupsEndpointTests : BaseFunctionalTest
    {
        public GetAllDiseaseGroupsEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetAllDiseaseGroups_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            var response = await _client.GetAsync("/disease-groups/all");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllDiseaseGroups_WithGroups_Returns200()
        {
            var response = await _client.GetAsync("/disease-groups/all");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var diseaseGroups = node?["diseaseGroups"]?.AsArray();

            Assert.NotNull(diseaseGroups);
            Assert.True(diseaseGroups.Count > 0);
        }

        [Theory]
        [InlineData("tim mạch")]
        public async Task GetAllDiseaseGroups_WithSearchTerm_ReturnsFilteredResults(string searchTerm)
        {
            var url = $"/disease-groups/all?searchTerm={searchTerm}";

            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            var diseaseGroups = node?["diseaseGroups"]?.AsArray();

            Assert.NotNull(diseaseGroups);
        }
    }
}
