using Appointment.API.Endpoints;

namespace AppointmentService.FunctionalTests.Tests
{
    public class GetAppointmentsTests : BaseFunctionalTest
    {
        private string _testToken;

        public GetAppointmentsTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAppointments_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            SetAuthHeader();

            // Act
            var response = await _client.GetAsync("/appointments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAppointmentsResponse>();
            result.Should().NotBeNull();
            result.Appointments.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAppointments_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Act
            var response = await _client.GetAsync("/appointments");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAppointments_ShouldReturnBadRequest_WhenPageIndexIsInvalid()
        {
            // Arrange
            SetAuthHeader();

            // Act
            var response = await _client.GetAsync("/appointments?pageIndex=0&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
