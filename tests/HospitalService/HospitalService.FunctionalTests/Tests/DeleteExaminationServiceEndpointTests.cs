using HospitalService.API.Endpoints;

namespace HospitalService.FunctionalTests.Tests
{
    public class DeleteExaminationServiceEndpointTests : BaseFunctionalTest
    {
        public DeleteExaminationServiceEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteExaminationService_WhenCalled_ReturnsSuccess()
        {
            // Arrange
            // Assuming there's a seeded examination service with ID 5
            var serviceId = 5;

            // Act
            var response = await _client.DeleteAsync($"/services/examination/{serviceId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadFromJsonAsync<DeleteExaminationServiceResponse>();

            content.Should().NotBeNull();
            content!.ServiceId.Should().Be(serviceId);
        }

        [Fact]
        public async Task DeleteExaminationService_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // BaseFunctionalTest adds auth by default, so we create a new client without it.
            _client.DefaultRequestHeaders.Remove("Authorization");
            var serviceId = 1;

            // Act
            var response = await _client.DeleteAsync($"/services/examination/{serviceId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task DeleteExaminationService_WithInvalidId_ReturnsBadRequest(int invalidId)
        {
            // Arrange
            // Act
            var response = await _client.DeleteAsync($"/services/examination/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}