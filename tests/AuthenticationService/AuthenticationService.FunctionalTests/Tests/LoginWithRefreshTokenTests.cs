using BuildingBlocks.Strings;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class LoginWithRefreshTokenTests : BaseFunctionalTest
    {
        private readonly FunctionalTestWebAppFactory _factory;
        private string _testToken;

        public LoginWithRefreshTokenTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
        {
            // Arrange
            SetAuthHeader();
            var refreshToken = "valid_refresh_token";

            // First verify token exists in database
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.RefreshTokens.AddAsync(new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = refreshToken,
                    UserId = 1,
                    Roles = Roles.ADMIN,
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
                });
                await dbContext.SaveChangesAsync();
            }

            var grpcResponse = new ApplicationUserDetailModel
            {
                Id = 1,
                UserName = "testuser",
                Departments = { new DepartmentSummaryModel { Name = "IT", NameInEnglish = "IT" } },
                Roles = Roles.ADMIN,
            };

            _grpcClientMock?
                .GetApplicationUserAsync(
                    Arg.Any<GetApplicationUserRequest>(),
                    Arg.Any<Metadata>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/login/refresh-token");
            request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LoginWithRefreshTokenResponse>();
            result.Should().NotBeNull();
            result?.IsSuccess.Should().BeTrue();
            result?.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var refreshToken = "invalid_token";

            var request = new HttpRequestMessage(HttpMethod.Post, "/login/refresh-token");
            request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RefreshToken_WithExpiredToken_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var refreshToken = "expired_token";
            var request = new HttpRequestMessage(HttpMethod.Post, "/login/refresh-token");
            request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

            var grpcResponse = new LoginResponse { IsSuccess = false, Message = "Token expired" };

            _grpcClientMock?
                .LoginAsync(Arg.Any<LoginRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RefreshToken_WithValidTokenButFailed_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var refreshToken = "valid_refresh_token_1";
            var request = new HttpRequestMessage(HttpMethod.Post, "/login/refresh-token");
            request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

            // First verify token exists in database
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.RefreshTokens.AddAsync(new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = refreshToken,
                    UserId = -1,
                    Roles = Roles.ADMIN,
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
                });
                await dbContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}
