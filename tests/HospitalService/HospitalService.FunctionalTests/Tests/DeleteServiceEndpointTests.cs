using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class DeleteServiceEndpointTests : BaseFunctionalTest
    {
        public DeleteServiceEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task DeleteService_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            var response = await _client.DeleteAsync("/services/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteService_InvalidId_Returns400()
        {
            var response = await _client.DeleteAsync("/services/0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteService_ValidId_Returns200()
        {
            var response = await _client.DeleteAsync("/services/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(json);
            Assert.NotNull(node);
            Assert.Equal(1, node["serviceId"]?.GetValue<int>());
        }
    }
}
