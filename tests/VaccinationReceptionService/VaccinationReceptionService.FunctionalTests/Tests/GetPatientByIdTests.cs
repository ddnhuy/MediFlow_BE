using BuildingBlocks.Strings;
using System.Text.Json;

namespace VaccinationReception.FunctionalTests.Tests;

public class GetPatientByIdTests : BaseFunctionalTest
{
    private readonly string _testToken;

    public GetPatientByIdTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task GetPatientById_WithValidId_ReturnsOkWithPatient()
    {
        // Arrange
        var patientId = 1;
        var grpcResponse = new PatientDetailModel
        {
            Id = patientId,
            Code = "BN001",
            Name = "Nguyen Van A",
            Gender = 1,
            Dob = Timestamp.FromDateTime(new DateTime(1990, 1, 1).ToUniversalTime()),
            PhoneNumber = "0123456789",
            IdentityCard = "123456789",
            AddressDetail = "123 Street",
            Province = "Hanoi",
            District = "Cau Giay",
            Ward = "Dich Vong",
            IsPregnant = false,
            IsForeigner = false,
            IsSuspended = false,
            IsCancelled = false
        };

        var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
            Task.FromResult(grpcResponse),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        _grpcClientMock?
            .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
            .Returns(asyncUnaryCall);

        // Act
        var response = await _client.GetAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetPatientByIdResponse>();
        result.Should().NotBeNull();
        result!.Patient.Should().NotBeNull();
        result.Patient.Id.Should().Be(patientId);
        result.Patient.Code.Should().Be("BN001");
        result.Patient.Name.Should().Be("Nguyen Van A");
    }

    [Fact]
    public async Task GetPatientById_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var response = await _client.GetAsync($"/patients/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientNotFound_ReturnsNotFound()
    {
        // Arrange
        var patientId = 999;
        var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
            Task.FromResult<PatientDetailModel>(null!),
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.NotFound, $"Patient with ID {patientId} not found"),
            () => new Metadata(),
            () => { });

        _grpcClientMock?
            .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
            .Throws(new RpcException(new Status(StatusCode.NotFound, $"Patient with ID {patientId} not found")));

        // Act
        var response = await _client.GetAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var errorString = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<ProblemDetails>(errorString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        error.Detail.Should().Be(ExceptionKey.NOT_FOUND_PATIENT_WITH_ID.ToString());

    }

    [Fact]
    public async Task GetPatientById_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var patientId = 1;

        _grpcClientMock?
            .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
            .Throws(new Exception("GRPC call failed"));

        // Act
        var response = await _client.GetAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
        error!.Detail.Should().Contain("GRPC call failed");
    }

    [Fact]
    public async Task GetPatientById_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var patientId = 1;
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync($"/patients/{patientId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}