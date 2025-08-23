namespace VaccinationReceptionService.FunctionalTests.Tests;

public class DeletePatientTests : BaseFunctionalTest
{
    private readonly string _testToken;

    public DeletePatientTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task DeletePatient_WithValidId_ReturnsOkWithSuccessTrue()
    {
        // Arrange
        var patientId = 1;
        var grpcResponse = new CustomerInfo.Grpc.Protos.DeletePatientResponse { IsSuccess = true };
        var asyncUnaryCall = new AsyncUnaryCall<CustomerInfo.Grpc.Protos.DeletePatientResponse>(
            Task.FromResult(grpcResponse),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        _grpcClientMock?
            .DeletePatientAsync(Arg.Any<DeletePatientRequest>(), Arg.Any<Metadata>())
            .Returns(asyncUnaryCall);

        // Act
        var response = await _client.DeleteAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CustomerInfo.Grpc.Protos.DeletePatientResponse>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePatient_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var patientId = 1;

        _grpcClientMock?
            .DeletePatientAsync(Arg.Any<DeletePatientRequest>(), Arg.Any<Metadata>())
            .Throws(new Exception("GRPC call failed"));

        // Act
        var response = await _client.DeleteAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
        error!.Detail.Should().Contain("GRPC call failed");
    }

    [Fact]
    public async Task DeletePatient_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var patientId = 1;
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.DeleteAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}