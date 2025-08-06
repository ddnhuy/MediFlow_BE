using HospitalService.API.Endpoints;
using HospitalService.Application.Services.HospitalServices.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class GetExaminationServiceByIdEndpointTests : BaseFunctionalTest
    {
        public GetExaminationServiceByIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetExaminationServiceById_WhenCalled_ReturnsSuccess()
        {
            // Arrange
            // Assuming there's a seeded examination service with ID 1
            var serviceId = 5;

            // Act
            var response = await _client.GetAsync($"/services/examination/{serviceId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<ExaminationServiceDetailDTO>();

            content.Should().NotBeNull();
            content!.Id.Should().Be(serviceId);
            content.ServiceType.Should().Be(BuildingBlocks.Strings.Enums.ServiceType.Test);
            content.ExaminationService.Should().NotBeNull();
        }

        [Fact]
        public async Task GetExaminationServiceById_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // BaseFunctionalTest adds auth by default, so we create a new client without it.
            _client.DefaultRequestHeaders.Remove("Authorization");
            var serviceId = 1;

            // Act
            var response = await _client.GetAsync($"/services/examination/{serviceId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetExaminationServiceById_WithInvalidId_ReturnsBadRequest(int invalidId)
        {
            // Arrange
            // Act
            var response = await _client.GetAsync($"/services/examination/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}