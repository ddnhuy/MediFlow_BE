using ListPatientsResponse = CustomerInfo.Grpc.Protos.ListPatientsResponse;
using DeletePatientResponse = CustomerInfo.Grpc.Protos.DeletePatientResponse;

namespace VaccinationReception.FunctionalTests.Tests;

public class PatientServiceTests : BaseFunctionalTest
{
    private readonly string _testToken;

    public PatientServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task ListPatients_WithValidPagination_ReturnsOk()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;
        var grpcResponse = new ListPatientsResponse
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItem = 2,
            Data = 
            {
                new PatientSummaryModel 
                { 
                    Id = 1,
                    Code = "BN100",
                    Name = "Test Patient 1"
                },
                new PatientSummaryModel 
                { 
                    Id = 2,
                    Code = "BN101",
                    Name = "Test Patient 2"
                }
            }
        };

        var asyncUnaryCall = new AsyncUnaryCall<ListPatientsResponse>(
            Task.FromResult(grpcResponse),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        _grpcClientMock?
            .ListPatientsAsync(Arg.Any<ListPatientsRequest>(), Arg.Any<Metadata>())
            .Returns(asyncUnaryCall);

        // Act
        var response = await _client.GetAsync($"/patients?pageIndex={pageIndex}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ListPatientsResponse>();
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPatient_WithValidId_ReturnsOk()
    {
        // Arrange
        var patientId = 1;
        var grpcResponse = new PatientDetailModel
        {
            Id = patientId,
            Code = "BN100",
            Name = "Test Patient",
            Gender = 1,
            Dob = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            PhoneNumber = "0123456789",
            IdentityCard = "123456789",
            Province = "Hanoi",
            District = "Cau Giay",
            Ward = "Mai Dich",
            AddressDetail = "123 Test St",
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
        var result = await response.Content.ReadFromJsonAsync<PatientDetailModel>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(patientId);
    }

    [Fact]
    public async Task CreatePatient_WithValidData_ReturnsCreated()
    {
        // Arrange
        var command = new CreatePatientCommand(
            Code: "BN102",
            Name: "New Patient",
            Gender: 1,
            Dob: new DateTime(1990, 1, 1),
            PhoneNumber: "0987654321",
            IdentityCard: "987654321",
            Province: "HCM",
            District: "District 1",
            Ward: "Ben Nghe",
            AddressDetail: "456 New St",
            IsPregnant: false,
            IsForeigner: false,
            IsSuspended: false,
            IsCancelled: false
        );

        var grpcResponse = new PatientDetailModel
        {
            Id = 3,
            Code = command.Code,
            Name = command.Name,
            Gender = command.Gender,
            Dob = Timestamp.FromDateTime(command.Dob.ToUniversalTime()),
            PhoneNumber = command.PhoneNumber,
            IdentityCard = command.IdentityCard,
            Province = command.Province,
            District = command.District,
            Ward = command.Ward,
            AddressDetail = command.AddressDetail,
            IsPregnant = command.IsPregnant,
            IsForeigner = command.IsForeigner,
            IsSuspended = command.IsSuspended,
            IsCancelled = command.IsCancelled
        };

        var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
            Task.FromResult(grpcResponse),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        _grpcClientMock?
            .CreatePatientAsync(Arg.Any<CreatePatientRequest>(), Arg.Any<Metadata>())
            .Returns(asyncUnaryCall);

        // Act
        var response = await _client.PostAsJsonAsync("/patients", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<PatientDetailModel>();
        result.Should().NotBeNull();
        result!.Code.Should().Be(command.Code);
    }

    [Fact]
    public async Task UpdatePatient_WithValidData_ReturnsOk()
    {
        // Arrange
        var command = new UpdatePatientCommand(
            Id: 1,
            Code: "BN100",
            Name: "Updated Patient",
            Gender: 1,
            Dob: new DateTime(1990, 1, 1),
            PhoneNumber: "0987654321",
            IdentityCard: "987654321",
            Province: "HCM",
            District: "District 2",
            Ward: "Ben Nghe",
            AddressDetail: "456 Updated St",
            IsPregnant: false,
            IsForeigner: false,
            IsSuspended: false,
            IsCancelled: false
        );

        var grpcResponse = new PatientDetailModel
        {
            Id = command.Id,
            Code = command.Code,
            Name = command.Name,
            Gender = command.Gender,
            Dob = Timestamp.FromDateTime(command.Dob.ToUniversalTime()),
            PhoneNumber = command.PhoneNumber,
            IdentityCard = command.IdentityCard,
            Province = command.Province,
            District = command.District,
            Ward = command.Ward,
            AddressDetail = command.AddressDetail,
            IsPregnant = command.IsPregnant,
            IsForeigner = command.IsForeigner,
            IsSuspended = command.IsSuspended,
            IsCancelled = command.IsCancelled
        };

        var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
            Task.FromResult(grpcResponse),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        _grpcClientMock?
            .UpdatePatientAsync(Arg.Any<UpdatePatientRequest>(), Arg.Any<Metadata>())
            .Returns(asyncUnaryCall);

        // Act
        var response = await _client.PutAsJsonAsync($"/patients/{command.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UpdatePatientResponse>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePatient_WithValidId_ReturnsOk()
    {
        // Arrange
        var patientId = 1;
        var grpcResponse = new DeletePatientResponse { IsSuccess = true };

        var asyncUnaryCall = new AsyncUnaryCall<DeletePatientResponse>(
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
        var result = await response.Content.ReadFromJsonAsync<DeletePatientResponse>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ListPatients_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var pageIndex = 1;
        var pageSize = 10;

        _grpcClientMock?
            .ListPatientsAsync(Arg.Any<ListPatientsRequest>(), Arg.Any<Metadata>())
            .Throws(new Exception("GRPC call failed"));

        // Act
        var response = await _client.GetAsync($"/patients?pageIndex={pageIndex}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
        error!.Detail.Should().Contain("GRPC call failed");
    }

    [Fact]
    public async Task GetPatient_WhenGrpcThrowsException_ReturnsInternalServerError()
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
    public async Task CreatePatient_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var command = new CreatePatientCommand(
            Code: "BN102",
            Name: "New Patient",
            Gender: 1,
            Dob: new DateTime(1990, 1, 1),
            PhoneNumber: "0987654321",
            IdentityCard: "987654321",
            Province: "DN",
            District: "District 2",
            Ward: "Ben Nghe",
            AddressDetail: "456 New St",
            IsPregnant: false,
            IsForeigner: false,
            IsSuspended: false,
            IsCancelled: false
        );

        _grpcClientMock?
            .CreatePatientAsync(Arg.Any<CreatePatientRequest>(), Arg.Any<Metadata>())
            .Throws(new Exception("GRPC call failed"));

        // Act
        var response = await _client.PostAsJsonAsync("/patients", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
        error!.Detail.Should().Contain("GRPC call failed");
    }

    [Fact]
    public async Task UpdatePatient_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var command = new UpdatePatientCommand(
            Id: 1,
            Code: "BN100",
            Name: "Updated Patient",
            Gender: 1,
            Dob: new DateTime(1990, 1, 1),
            PhoneNumber: "0987654321",
            IdentityCard: "987654321",
            Province: "HCM",
            District: "District 3",
            Ward: "Ben Nghe",
            AddressDetail: "456 Updated St",
            IsPregnant: false,
            IsForeigner: false,
            IsSuspended: false,
            IsCancelled: false
        );

        _grpcClientMock?
            .UpdatePatientAsync(Arg.Any<UpdatePatientRequest>(), Arg.Any<Metadata>())
            .Throws(new Exception("GRPC call failed"));

        // Act
        var response = await _client.PutAsJsonAsync($"/patients/{command.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        error.Should().NotBeNull();
        error!.Detail.Should().Contain("GRPC call failed");
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
} 