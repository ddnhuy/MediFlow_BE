using HospitalService.API.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetAllExaminationServicesEndpointTests : BaseFunctionalTest
    {
        public GetAllExaminationServicesEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllExaminationServices_WhenCalled_ReturnsSuccess()
        {
            // Arrange
            // Data is seeded by FunctionalTestWebAppFactory

            // Act
            var response = await _client.GetAsync("/services/examination");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<GetAllExaminationServicesResponse>();

            content.Should().NotBeNull();
            content!.Services.Should().NotBeNull();

            // Verify that all returned services are examination services
            if (content.Services.Any())
            {
                content.Services.Should().OnlyContain(s => s.ServiceType == BuildingBlocks.Strings.Enums.ServiceType.Test);
                content.Services.Should().OnlyContain(s => s.ExaminationService.HasValue);
            }
        }

        [Fact]
        public async Task GetAllExaminationServices_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // BaseFunctionalTest adds auth by default, so we create a new client without it.
            _client.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await _client.GetAsync("/services/examination");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}