namespace VaccinationReception.FunctionalTests.Tests;

public class UpdatePatientTests : BaseFunctionalTest
{
    private readonly string _testToken;

    public UpdatePatientTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        _testToken = TokenHelper.GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
    }

    [Fact]
    public async Task UpdatePatient_WithValidData_ReturnsOkWithSuccessTrue()
    {
        // Arrange
        var command = new UpdatePatientCommand(
            Id: 1,
            Code: "BN100",
            Name: "Nguyen Van B",
            Gender: 1,
            Dob: new DateTime(1991, 2, 2),
            PhoneNumber: "0999999999",
            IdentityCard: "555555555",
            Province: "Hanoi",
            District: "Cau Giay",
            Ward: "Mai Dich",
            AddressDetail: "456 Main St",
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
            AddressDetail = command.AddressDetail,
            Province = command.Province,
            District = command.District,
            Ward = command.Ward,
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
    public async Task UpdatePatient_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var command = new UpdatePatientCommand(
            Id: 2,
            Code: "BN101",
            Name: "Le Thi C",
            Gender: 0,
            Dob: new DateTime(1992, 3, 3),
            PhoneNumber: "0888888888",
            IdentityCard: "444444444",
            Province: "HCM",
            District: "1",
            Ward: "Ben Nghe",
            AddressDetail: "789 A St",
            IsPregnant: false,
            IsForeigner: false,
            IsSuspended: false,
            IsCancelled: false
        );

        // Act
        var response = await _client.PutAsJsonAsync("/patients/99", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePatient_WhenGrpcReturnsFailure_ReturnsOkWithSuccessFalse()
    {
        // Arrange
        var command = new UpdatePatientCommand(
            Id: 3,
            Code: "BN102",
            Name: "Tran Van D",
            Gender: 1,
            Dob: new DateTime(1993, 4, 4),
            PhoneNumber: "0777777777",
            IdentityCard: "333333333",
            Province: "Danang",
            District: "Hai Chau",
            Ward: "Thanh Binh",
            AddressDetail: "101 River Rd",
            IsPregnant: false,
            IsForeigner: false,
            IsSuspended: false,
            IsCancelled: false
        );

        var grpcResponse = new PatientDetailModel { Id = command.Id };
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
    public async Task UpdatePatient_WhenGrpcThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var command = new UpdatePatientCommand(
            Id: 5,
            Code: "BN104",
            Name: "Do Thi F",
            Gender: 0,
            Dob: new DateTime(1995, 6, 6),
            PhoneNumber: "0555555555",
            IdentityCard: "111111111",
            Province: "Can Tho",
            District: "Ninh Kieu",
            Ward: "An Cu",
            AddressDetail: "67 Mekong St",
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
}