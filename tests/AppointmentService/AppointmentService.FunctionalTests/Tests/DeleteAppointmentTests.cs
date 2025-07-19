using Appointment.API.Endpoints;
using BuildingBlocks.Strings.Enums;

namespace AppointmentService.FunctionalTests.Tests
{
    public class DeleteAppointmentTests : BaseFunctionalTest
    {
        private string _testToken;

        public DeleteAppointmentTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task DeleteAppointment_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            SetAuthHeader();
            var create_request = new CreateAppointmentRequest(1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "PATIENT1", "Patient 1", DateTime.UtcNow.AddDays(-1), "patient@example.com", "84123456789", "Influenza", null, 1, 1, "");
            await _client.PostAsJsonAsync("/", create_request);

            // Act
            var response = await _client.DeleteAsync("/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DeleteAppointmentResponse>();
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAppointment_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            // Arrange
            SetAuthHeader();

            // Act
            var response = await _client.DeleteAsync("/0");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteAppointment_ShouldReturnUnauthorized_WhenNoAuthHeader()
        {
            // Act
            var response = await _client.DeleteAsync("/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
