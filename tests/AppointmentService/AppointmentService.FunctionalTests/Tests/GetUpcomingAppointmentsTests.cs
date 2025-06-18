using Appointment.API.Endpoints;

namespace AppointmentService.FunctionalTests.Tests
{
    public class GetUpcomingAppointmentsTests : BaseFunctionalTest
    {
        private string _testToken;

        public GetUpcomingAppointmentsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetUpcomingAppointments_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            SetAuthHeader();

            // Act
            var response = await _client.GetAsync("/appointments/upcoming");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetUpcomingAppointmentsResponse>();
            result.Should().NotBeNull();
            result.Appointments.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUpcomingAppointments_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Act
            var response = await _client.GetAsync("/appointments/upcoming");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
