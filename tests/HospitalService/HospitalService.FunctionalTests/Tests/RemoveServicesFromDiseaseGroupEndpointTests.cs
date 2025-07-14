using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class RemoveServicesFromDiseaseGroupEndpointTests : BaseFunctionalTest
    {
        public RemoveServicesFromDiseaseGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task RemoveServicesFromDiseaseGroup_Unauthorized_Returns401()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            var requestBody = JsonContent.Create(new { ServiceIds = new[] { 1, 2 } });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/disease-groups/1/services", UriKind.Relative),
                Content = requestBody
            };

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RemoveServicesFromDiseaseGroup_InvalidRequest_Returns400()
        {
            var requestBody = JsonContent.Create(new { ServiceIds = new int[] { } });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/disease-groups/1/services", UriKind.Relative),
                Content = requestBody
            };

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoveServicesFromDiseaseGroup_ValidRequest_Returns200()
        {
            var requestBody = JsonContent.Create(new { ServiceIds = new[] { 1, 2 } });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/disease-groups/1/services", UriKind.Relative),
                Content = requestBody
            };

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
