using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetDiseaseGroupsEndpointTests : BaseFunctionalTest
    {
        public GetDiseaseGroupsEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task GetDiseaseGroups_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            var response = await _client.GetAsync("/disease-groups");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 10)]
        [InlineData(1, -1)]
        public async Task GetDiseaseGroups_InvalidPagination_Returns400(int pageIndex, int pageSize)
        {
            var url = $"/disease-groups?PageIndex={pageIndex}&PageSize={pageSize}";

            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetDiseaseGroups_ValidRequest_Returns200()
        {
            var url = "/disease-groups?PageIndex=1&PageSize=10";

            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);

            var data = node?["diseaseGroups"]?["data"]?.AsArray();

            Assert.NotNull(data);
            Assert.True(data.Count > 0);
        }
    }
}
