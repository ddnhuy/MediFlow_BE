using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class RemoveServicesFromGroupEndpointTests : BaseFunctionalTest
    {
        public RemoveServicesFromGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task RemoveServicesFromGroup_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            var requestBody = JsonContent.Create(new { ServiceIds = new[] { 1, 2 } });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/service-groups/1/services", UriKind.Relative),
                Content = requestBody
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RemoveServicesFromGroup_InvalidRequest_Returns400()
        {
            // Arrange
            var requestBody = JsonContent.Create(new { ServiceIds = new int[] { } }); // Empty array invalid

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/service-groups/1/services", UriKind.Relative),
                Content = requestBody
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoveServicesFromGroup_ValidRequest_Returns200()
        {
            // Arrange
            var requestBody = JsonContent.Create(new { ServiceIds = new[] { 1, 2 } });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("/service-groups/1/services", UriKind.Relative),
                Content = requestBody
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
