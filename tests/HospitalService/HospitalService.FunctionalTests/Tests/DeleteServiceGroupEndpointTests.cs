using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class DeleteServiceGroupEndpointTests : BaseFunctionalTest
    {
        public DeleteServiceGroupEndpointTests(FunctionalTestWebAppFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task DeleteServiceGroup_Unauthorized_Returns401()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.DeleteAsync("/servicegroups/1");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteServiceGroup_InvalidId_Returns400()
        {
            // Arrange
            var invalidId = 0; // Invalid ID

            // Act
            var response = await _client.DeleteAsync($"/servicegroups/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteServiceGroup_ValidRequest_Returns200()
        {
            // Arrange
            var validId = 1;

            // Act
            var response = await _client.DeleteAsync($"/servicegroups/{validId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
