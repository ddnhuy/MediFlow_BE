using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class LoginWithRefreshTokenTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;

        public LoginWithRefreshTokenTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var request = new LoginWithRefreshTokenRequest(refreshToken);

            // First verify token exists in database
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.RefreshTokens.AddAsync(new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = refreshToken,
                    UserId = 1,
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
                });
                await dbContext.SaveChangesAsync();
            }

            var grpcResponse = new ApplicationUserDetailModel
            {
                Id = 1,
                UserName = "testuser",
                Departments = { new DepartmentSummaryModel { Name = "IT" } }
            };

            _grpcClientMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/login/refresh-token",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LoginWithRefreshTokenResponse>();
            result.Should().NotBeNull();
            result?.AccessToken.Should().NotBeNullOrEmpty();
            result?.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginWithRefreshTokenRequest("invalid_token");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/login/refresh-token",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RefreshToken_WithExpiredToken_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginWithRefreshTokenRequest("expired_token");
            var grpcResponse = new LoginResponse { IsSuccess = false, Message = "Token expired" };

            _grpcClientMock?
                .LoginAsync(Arg.Any<LoginRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/login/refresh-token",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RefreshToken_WithValidTokenButFailed_ReturnsBadRequest()
        {
            // Arrange
            var refreshToken = "valid_refresh_token_1";
            var request = new LoginWithRefreshTokenRequest(refreshToken);

            // First verify token exists in database
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.RefreshTokens.AddAsync(new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = refreshToken,
                    UserId = -1,
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
                });
                await dbContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.PostAsJsonAsync(
                "/login/refresh-token",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}
