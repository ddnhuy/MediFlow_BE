using Appointment.API.Endpoints;

namespace AppointmentService.FunctionalTests.Tests
{
    public class GetAppointmentsByPatientIdTests : BaseFunctionalTest
    {
        private string _testToken;

        public GetAppointmentsByPatientIdTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAppointmentsByPatientId_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            SetAuthHeader();
            var patientId = "1";

            // Act
            var response = await _client.GetAsync($"/patients/{patientId}/appointments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAppointmentsByPatientIdResponse>();
            result.Should().NotBeNull();
            result.Appointments.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAppointmentsByPatientId_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var patientId = "1";

            // Act
            var response = await _client.GetAsync($"/patients/{patientId}/appointments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
