using Appointment.API.Endpoints;
using BuildingBlocks.Strings.Enums;

namespace AppointmentService.FunctionalTests.Tests
{
    public class UpdateAppointmentTests : BaseFunctionalTest
    {
        private string _testToken;

        public UpdateAppointmentTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task UpdateAppointment_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            SetAuthHeader();
            var create_request = new CreateAppointmentRequest(1, 1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "patient@example.com", "84123456789", null);
            await _client.PostAsJsonAsync("/", create_request);

            var request = new UpdateAppointmentRequest(1, 1, 1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "patient@example.com", "84123456789", null, false);

            // Act
            var response = await _client.PutAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateAppointmentResponse>();
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAppointment_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new UpdateAppointmentRequest(1, 1, 1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "invalid-email", "84123456789", null, false);

            // Act
            var response = await _client.PutAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateAppointment_ShouldReturnUnauthorized_WhenNoAuthHeader()
        {
            // Arrange
            var request = new UpdateAppointmentRequest(1, 1, 1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "patient@example.com", "84123456789", null, false);

            // Act
            var response = await _client.PutAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task UpdateAppointment_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            SetAuthHeader();
            var request = new UpdateAppointmentRequest(999, 1, 1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "patient@example.com", "84123456789", null, false);

            // Act
            var response = await _client.PutAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
