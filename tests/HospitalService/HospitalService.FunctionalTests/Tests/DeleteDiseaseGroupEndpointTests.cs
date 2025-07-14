using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class DeleteDiseaseGroupEndpointTests : BaseFunctionalTest
    {
        public DeleteDiseaseGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task DeleteDiseaseGroup_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");

            var response = await _client.DeleteAsync("/disease-groups/1");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDiseaseGroup_InvalidId_Returns400()
        {
            var invalidId = 0;

            var response = await _client.DeleteAsync($"/disease-groups/{invalidId}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDiseaseGroup_ValidRequest_Returns200()
        {
            var validId = 1;

            var response = await _client.DeleteAsync($"/disease-groups/{validId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDiseaseGroup_NotFoundGroup_Returns404()
        {
            var response = await _client.DeleteAsync($"/disease-groups/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
