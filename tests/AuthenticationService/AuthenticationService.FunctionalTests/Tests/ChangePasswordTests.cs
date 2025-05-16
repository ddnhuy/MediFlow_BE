using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AuthenticationService.FunctionalTests.Tests
{
    public class ChangePasswordTests : BaseFunctionalTest
    {
        private string _testToken;

        public ChangePasswordTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }

        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task ChangePassword_WithValidCredentials_ReturnsSuccessMessage()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ChangePasswordRequest(1, "Mediflow@123", "Mediflow@1234");

            var grpcResponse = new HumanResource.Grpc.ChangePasswordResponse
            {
                IsSuccess = true,
                Message = HumanResourceSuccessStrings.SUCCESS_CHANGE_PASSWORD
            };

            _grpcClientMock?
                .ChangePasswordAsync(Arg.Any<HumanResource.Grpc.ChangePasswordRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/change-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Authentication.API.Endpoints.ChangePasswordResponse>();
            result.Should().NotBeNull();
            result?.IsSuccess.Should().BeTrue();
            result?.Message.Should().Be(HumanResourceSuccessStrings.SUCCESS_CHANGE_PASSWORD);
        }

        [Fact]
        public async Task ChangePassword_WithWrongCurrentPassword_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ChangePasswordRequest(1, "Mediflow@123", "Mediflow@1234");

            var grpcResponse = new HumanResource.Grpc.ChangePasswordResponse
            {
                IsSuccess = false,
                Message = HumanResourceExceptionStrings.FAILED_CHANGE_PASSWORD
            };

            _grpcClientMock?
                .ChangePasswordAsync(Arg.Any<HumanResource.Grpc.ChangePasswordRequest>())
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var response = await _client.PostAsJsonAsync(
                "/change-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result?.Detail.Should().Be(HumanResourceExceptionStrings.FAILED_CHANGE_PASSWORD);
        }

        [Fact]
        public async Task ChangePassword_WithSameNewAndCurrentPassword_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ChangePasswordRequest(1, "Mediflow@123", "Mediflow@123");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/change-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ChangePassword_WithInvalidPasswordLength_ReturnsBadRequest()
        {
            // Arrange
            SetAuthHeader();
            var request = new Authentication.API.Endpoints.ChangePasswordRequest(1, "Med@123", "Mediflow@123");

            // Act
            var response = await _client.PostAsJsonAsync(
                "/change-password",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}
