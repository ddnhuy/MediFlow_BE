using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class LoginWithUserNameTests : BaseFunctionalTest
    {
        public LoginWithUserNameTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokens()
        {
            // Arrange
            var request = new LoginWithUserNameRequest("testuser", "password");
            var grpcResponse = new LoginResponse
            {
                IsSuccess = true,
                User = new ApplicationUserDetailModel
                {
                    Id = 1,
                    UserName = "testuser",
                    Departments = { new DepartmentSummaryModel { Name = "IT" } }
                }
            };

            _grpcClientMock?
                .LoginAsync(Arg.Any<LoginRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync("/login", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LoginWithUserNameResponse>();
            result.Should().NotBeNull();
            result?.AccessToken.Should().NotBeNullOrEmpty();
            result?.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginWithUserNameRequest(string.Empty, "wrong");
            var grpcResponse = new LoginResponse { IsSuccess = false, Message = "Invalid credentials" };

            _grpcClientMock?
                .LoginAsync(Arg.Any<LoginRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync("/login", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginWithUserNameRequest("invalid", "wrong");
            var grpcResponse = new LoginResponse { IsSuccess = false, Message = "Invalid credentials" };

            _grpcClientMock?
                .LoginAsync(Arg.Any<LoginRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync("/login", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WithValidCredentialsButFailed_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginWithUserNameRequest("testuser", "password");
            var grpcResponse = new LoginResponse
            {
                IsSuccess = false,
                Message = HumanResourceExceptionStrings.INVALID_LOGIN_CREDENTIAL
            };

            _grpcClientMock?
                .LoginAsync(Arg.Any<LoginRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync("/login", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}