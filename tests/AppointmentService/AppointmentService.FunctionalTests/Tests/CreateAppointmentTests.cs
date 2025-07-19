using Appointment.API.Endpoints;
using BuildingBlocks.Strings.Enums;

namespace AppointmentService.FunctionalTests.Tests
{
    public class CreateAppointmentTests : BaseFunctionalTest
    {
        private string _testToken;

        public CreateAppointmentTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task CreateAppointment_ShouldReturnCreated_WhenValidRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new CreateAppointmentRequest(1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "PATIENT1", "Patient 1", DateTime.UtcNow.AddDays(-1), "patient@example.com", "84123456789", "COVID-19", null, 1, 1, "");

            // Act
            var response = await _client.PostAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateAppointment_ShouldReturnBadRequest_WhenInvalidRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new CreateAppointmentRequest(0, DateTime.UtcNow.AddDays(-1), AppointmentType.Vaccination, "PATIENT1", "Patient 1", DateTime.UtcNow.AddDays(-1), "invalid-email", "84123456789", null, null, 1, 1, "");

            // Act
            var response = await _client.PostAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateAppointment_ShouldReturnUnauthorized_WhenNoAuthHeader()
        {
            // Arrange
            var request = new CreateAppointmentRequest(1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "PATIENT1", "Patient 1", DateTime.UtcNow.AddDays(-1), "patient@example.com", "84123456789", null, null, 1, 1, "");

            // Act
            var response = await _client.PostAsJsonAsync("/", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
