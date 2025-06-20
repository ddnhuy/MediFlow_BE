using BuildingBlocks.Strings;

namespace VaccinationReception.FunctionalTests.Tests
{
    public class CreatePatientTests : BaseFunctionalTest
    {
        private readonly string _testToken;

        public CreatePatientTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task CreatePatient_WithValidData_ReturnsCreatedResponse()
        {
            // Arrange
            var command = new CreatePatientCommand(
                Code: "BN001",
                Name: "Nguyen Van A",
                Gender: 1,
                Dob: new DateTime(1990, 1, 1),
                PhoneNumber: "0123456789",
                IdentityCard: "123456789",
                Province: "Hanoi",
                District: "Cau Giay",
                Ward: "Dich Vong",
                AddressDetail: "123 Street",
                IsPregnant: false,
                IsForeigner: false,
                IsSuspended: false,
                IsCancelled: false
            );

            var grpcResponse = new PatientDetailModel { Id = 123 };
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
            var result = await response.Content.ReadFromJsonAsync<CreatePatientResponse>();
            result.Should().NotBeNull();
            result!.Id.Should().Be(123);
        }

        [Fact]
        public async Task CreatePatient_WhenGrpcReturnsNull_ThrowsInternalServerException()
        {
            // Arrange
            var command = new CreatePatientCommand(
                Code: "BN002",
                Name: "Tran Thi B",
                Gender: 0,
                Dob: new DateTime(1995, 5, 5),
                PhoneNumber: "0987654321",
                IdentityCard: "987654321",
                Province: "Hanoi",
                District: "Cau Giay",
                Ward: "Dich Vong",
                AddressDetail: "456 Street",
                IsPregnant: false,
                IsForeigner: false,
                IsSuspended: false,
                IsCancelled: false
            );

            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult<PatientDetailModel>(null!),
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
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            error.Should().NotBeNull();
            error?.Detail.Should().Contain(ExceptionKey.FAILED_CREATE_PATIENT.ToString());
        }

        [Fact]
        public async Task CreatePatient_WhenGrpcThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var command = new CreatePatientCommand(
                Code: "BN003",
                Name: "Le Van C",
                Gender: 1,
                Dob: new DateTime(1985, 10, 10),
                PhoneNumber: "0912345678",
                IdentityCard: "456789123",
                Province: "Hanoi",
                District: "Cau Giay",
                Ward: "Dich Vong",
                AddressDetail: "789 Street",
                IsPregnant: false,
                IsForeigner: false,
                IsSuspended: false,
                IsCancelled: false
            );

            _grpcClientMock?
                .CreatePatientAsync(Arg.Any<CreatePatientRequest>(), Arg.Any<Metadata>())
                .Throws(new Exception("Some internal error"));

            // Act
            var response = await _client.PostAsJsonAsync("/patients", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            error.Should().NotBeNull();
            error?.Detail.Should().Contain("Some internal error");
        }
        [Fact]
        public async Task CreatePatient_WhenHandlerReturnsNull_ThrowsInternalServerException()
        {
            // Arrange
            var command = new CreatePatientCommand(
                Code: "BN004",
                Name: "Test Null",
                Gender: 1,
                Dob: new DateTime(2000, 1, 1),
                PhoneNumber: "0000000000",
                IdentityCard: "000000000",
                Province: "Test",
                District: "Test",
                Ward: "Test",
                AddressDetail: "Test",
                IsPregnant: false,
                IsForeigner: false,
                IsSuspended: false,
                IsCancelled: false
            );

            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult<PatientDetailModel>(null!),
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
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var error = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            error.Should().NotBeNull();
            error?.Detail.Should().Contain(ExceptionKey.FAILED_CREATE_PATIENT.ToString());
        }
    }
}