using BuildingBlocks.Strings;
using HospitalService.Application.Services.HospitalServices.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetServicesByGroupEndpointTests : BaseFunctionalTest
    {
        public GetServicesByGroupEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetServicesByGroup_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Remove("Authorization");
            // Act
            var response = await _client.GetAsync("/services/group?groupId=1&groupType=ServiceGroup");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(0, "ServiceGroup")]
        [InlineData(-1, "DiseaseGroup")]
        public async Task GetServicesByGroup_WithInvalidGroupId_ReturnsBadRequest(int groupId, string groupType)
        {
            // Arrange
            var url = $"/services/group?groupId={groupId}&groupType={groupType}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(1, "InvalidType")]
        [InlineData(1, "")]
        [InlineData(1, " ")]
        public async Task GetServicesByGroup_WithInvalidGroupType_ReturnsBadRequest(int groupId, string groupType)
        {
            // Arrange
            var url = $"/services/group?groupId={groupId}&groupType={groupType}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetServicesByGroup_ForExistingServiceGroup_ReturnsOkWithServices()
        {
            // Arrange
            // Assumes seeder creates a ServiceGroup with Id=1 and associated services
            var groupId = 1;
            var groupType = GroupServiceType.SERVICE_GROUP;
            var url = $"/services/group?groupId={groupId}&groupType={groupType}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<List<GetServicesByGroupResponse>>();
            content.Should().NotBeNull();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetServicesByGroup_ForNonExistentGroup_ReturnsOkWithEmptyList()
        {
            // Arrange
            // Assumes a group with Id 999 does not exist
            var groupId = 999;
            var groupType = GroupServiceType.SERVICE_GROUP;
            var url = $"/services/group?groupId={groupId}&groupType={groupType}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<List<GetServicesByGroupResponse>>();
            content.Should().NotBeNull();
            content.Should().BeEmpty();
        }
    }
}
