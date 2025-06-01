namespace ManagementService.FunctionalTests.Tests;

public class RolesControllerTest : BaseFunctionalTest
{
    private string _testToken;

    public RolesControllerTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
    }

    private void SetAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task CallApi_NotAuthenticated_ReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/departments");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRoles_ReturnsSuccess()
    {
        // Arrange
        SetAuthHeader();

        var grpcResponse = new HumanResource.Grpc.ListRoleNamesResponse
        {
            RoleNames = { Roles.ADMIN, Roles.ACCOUNTANT, Roles.IT_SUPPORT }
        };

        _grpcRoleClientMock?
            .ListRoleNamesAsync(
                Arg.Any<HumanResource.Grpc.ListRoleNamesRequest>(),
                Arg.Any<Metadata>(),
                Arg.Any<DateTime?>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

        // Act
        var response = await _client.GetAsync("/roles/names");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetRolesResult>();
        result.Should().NotBeNull();
    }
}
