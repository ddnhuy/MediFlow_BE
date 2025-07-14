using HospitalService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetServicesByIdsEndpointTests : BaseFunctionalTest
    {
        public GetServicesByIdsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task GetServicesByIds_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");

            var requestBody = new List<int> { 1 };

            // Act
            var response = await _client.PostAsJsonAsync("/services/by-ids", requestBody);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [MemberData(nameof(InvalidRequestData))]
        public async Task GetServicesByIds_WithInvalidData_ReturnsBadRequest(List<int> serviceIds)
        {
            // Act
            var response = await _client.PostAsJsonAsync("/services/by-ids", serviceIds);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public static IEnumerable<object[]> InvalidRequestData()
        {
            yield return new object[] { new List<int>() }; // Empty list
            yield return new object[] { new List<int> { 1, 0, 2 } }; // Contains zero
            yield return new object[] { new List<int> { -1, 2, 3 } }; // Contains negative ID
        }

        [Fact]
        public async Task GetServicesByIds_WithValidIds_ReturnsOkWithServices()
        {
            // Arrange
            // Assuming seeder creates services with Ids 1 and 2, but not 999.
            var requestBody = new List<int> { 1, 2, 999 };

            // Act
            var response = await _client.PostAsJsonAsync("/services/by-ids", requestBody);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<List<ServiceDTO>>();

            content.Should().NotBeNull();
            content.Should().HaveCount(2); // Only returns existing services
            content.Should().Contain(s => s.Id == 1);
            content.Should().Contain(s => s.Id == 2);
            content.Should().NotContain(s => s.Id == 999);
        }
    }
}
