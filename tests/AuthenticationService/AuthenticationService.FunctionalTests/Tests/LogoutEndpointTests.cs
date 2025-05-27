namespace AuthenticationService.FunctionalTests.Tests
{
    public class LogoutEndpointTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;

        public LogoutEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Logout_ShouldClearCookiesAndReturnSuccess()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

            // Act
            var response = await client.PostAsync("/logout", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var logoutResponse = await response.Content.ReadFromJsonAsync<LogoutResponse>();

            logoutResponse.Should().NotBeNull();
            logoutResponse!.IsSuccess.Should().BeTrue();
            logoutResponse.Message.Should().Be("Đăng xuất thành công.");

            // Check cookies
            var setCookieHeaders = response.Headers
                .Where(h => h.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                .SelectMany(h => h.Value)
                .ToList();

            // Verify access_token cookie is cleared and expired
            setCookieHeaders.Should().Contain(c => c.Contains("access_token=") && c.Contains("expires="));

            // Verify refresh_token cookie is cleared and expired
            setCookieHeaders.Should().Contain(c => c.Contains("refresh_token=") && c.Contains("expires="));
        }
    }
}
