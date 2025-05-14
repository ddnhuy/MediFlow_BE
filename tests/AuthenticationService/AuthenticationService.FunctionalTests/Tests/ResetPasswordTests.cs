using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class ResetPasswordTests : BaseFunctionalTest
    {
        public ResetPasswordTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ResetPassword_WithValidCredentials_ReturnsSuccessMessage()
        {
            // Arrange
            var request = new Authentication.API.Endpoints.ResetPasswordRequest("admin@mediflow.health.vn");

            var grpcResponse = new HumanResource.Grpc.ResetPasswordResponse
            {
                IsSuccess = true,
                Message = HumanResourceSuccessStrings.SUCCESS_RESET_PASSWORD
            };

            _grpcClientMock?
                .ResetPasswordAsync(Arg.Any<HumanResource.Grpc.ResetPasswordRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/authentication/reset-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Authentication.API.Endpoints.ResetPasswordResponse>();
            result.Should().NotBeNull();
            result?.IsSuccess.Should().BeTrue();
            result?.Message.Should().Be(HumanResourceSuccessStrings.SUCCESS_RESET_PASSWORD);
        }

        [Fact]
        public async Task ResetPassword_WithWrongEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new Authentication.API.Endpoints.ResetPasswordRequest("adminmediflow.health.vn");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/authentication/reset-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ResetPassword_WithEmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new Authentication.API.Endpoints.ResetPasswordRequest(string.Empty);

            // Act
            var response = await _client.PostAsJsonAsync(
                "/authentication/reset-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}
