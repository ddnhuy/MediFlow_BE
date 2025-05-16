using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class RevokeRefreshTokenTests : BaseFunctionalTest
    {
        private string _testToken;

        public RevokeRefreshTokenTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        [Fact]
        public async Task RevokeRefreshToken_WithValidCredentials_ReturnsSuccessMessage()
        {
            // Arrange
            SetAuthHeader();
            var request = new RevokeRefreshTokenRequest(GenerateRefreshToken());

            // Act
            var response = await _client.PostAsJsonAsync(
                "/revoke-refresh-token",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<RevokeRefreshTokenResponse>();
            result.Should().NotBeNull();
            result?.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task RevokeRefreshToken_WithoutAuthorization_ReturnsBadRequest()
        {
            // Arrange
            var request = new RevokeRefreshTokenRequest(GenerateRefreshToken());

            // Act
            var response = await _client.PostAsJsonAsync(
                "/revoke-refresh-token",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
