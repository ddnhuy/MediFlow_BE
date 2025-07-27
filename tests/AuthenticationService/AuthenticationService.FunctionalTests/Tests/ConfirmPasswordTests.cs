using BuildingBlocks.Strings;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class ConfirmPasswordTests : BaseFunctionalTest
    {
        private string _testToken;

        public ConfirmPasswordTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task ConfirmPassword_WithValidPassword_ReturnsSuccessMessage()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ConfirmPasswordRequest("Mediflow@123");

            var grpcResponse = new HumanResource.Grpc.ConfirmPasswordResponse
            {
                IsSuccess = true,
                Message = "Password is correct."
            };

            _grpcClientMock?
                .ConfirmPasswordAsync(
                    Arg.Any<HumanResource.Grpc.ConfirmPasswordRequest>(),
                    Arg.Any<Metadata>(),
                    Arg.Any<DateTime?>(),
                    Arg.Any<CancellationToken>()
                )
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/confirm-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Authentication.API.Endpoints.ConfirmPasswordResponse>();
            result.Should().NotBeNull();
            result?.IsSuccess.Should().BeTrue();
            result?.Message.Should().Be("Password is correct.");
        }

        [Fact]
        public async Task ConfirmPassword_WithInvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ConfirmPasswordRequest("WrongPassword123");

            var grpcResponse = new HumanResource.Grpc.ConfirmPasswordResponse
            {
                IsSuccess = false,
                Message = "Password is incorrect."
            };

            _grpcClientMock?
                .ConfirmPasswordAsync(
                    Arg.Any<HumanResource.Grpc.ConfirmPasswordRequest>(),
                    Arg.Any<Metadata>(),
                    Arg.Any<DateTime?>(),
                    Arg.Any<CancellationToken>()
                )
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/confirm-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Authentication.API.Endpoints.ConfirmPasswordResponse>();
            result.Should().NotBeNull();
            result?.IsSuccess.Should().BeFalse();
            result?.Message.Should().Be("Password is incorrect.");
        }

        [Fact]
        public async Task ConfirmPassword_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            // No auth header set
            var request = new Authentication.API.Endpoints.ConfirmPasswordRequest("Mediflow@123");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/confirm-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ConfirmPassword_WithEmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ConfirmPasswordRequest("");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/confirm-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ConfirmPassword_WithShortPassword_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ConfirmPasswordRequest("123");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/confirm-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ConfirmPassword_WithNullPassword_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ConfirmPasswordRequest(null!);

            // Act
            var response = await _client.PostAsJsonAsync(
                "/confirm-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}