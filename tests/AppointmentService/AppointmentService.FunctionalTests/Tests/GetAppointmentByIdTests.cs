using Appointment.API.Endpoints;
using BuildingBlocks.Strings.Enums;
using CustomerInfo.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;

namespace AppointmentService.FunctionalTests.Tests
{
    public class GetAppointmentByIdTests : BaseFunctionalTest
    {
        private string _testToken;
        public GetAppointmentByIdTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _testToken = TokenHelper.GenerateTestToken();
        }
        private void SetAuthHeader()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _testToken);
        }

        [Fact]
        public async Task GetAppointmentById_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            SetAuthHeader();
            var create_request = new CreateAppointmentRequest(1, DateTime.UtcNow.AddDays(1), AppointmentType.Vaccination, "PATIENT1", "Patient 1", DateTime.UtcNow.AddDays(-1), "patient@example.com", "84123456789", null, null, 1, 1, "");
            await _client.PostAsJsonAsync("/", create_request);

            var appointmentId = "1";
            var userId = 1;
            var departmentId = 1;
            var patientId = 1;
            
            var grpcResponse = new HumanResource.Grpc.ApplicationUserDetailModel
            {
                Id = userId,
                Code = "TD001",
                Name = "Test User",
                UserName = "testuser",
                Email = "testemail@gmail.com",
                Roles = Roles.ADMIN,
                IsSuspended = false,
                IsCancelled = false,
                CreatedBy = 1,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                LastUpdatedBy = 1,
                LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                Address = "123 Test St, Test City",
                Departments = { new HumanResource.Grpc.DepartmentSummaryModel
                {
                    Id = 1,
                    Code = "TD001",
                    Name = "Test Department",
                    NameInEnglish = "Test Department EN",
                    DepartmentTypeName = "Test Type",
                    DepartmentTypeNameInEnglish = "Test Type EN",
                    IsSuspended = false,
                } },
                ProfilePictureUrl = "https://example.com/profile.jpg",
            };

            _grpcUserClientMock?
                .GetApplicationUserAsync(
                    Arg.Any<HumanResource.Grpc.GetApplicationUserRequest>(),
                    Arg.Any<Metadata>(),
                    Arg.Any<DateTime?>(),
                    Arg.Any<CancellationToken>()
                )
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcResponse));

            var grpcDepartmentResponse = new HumanResource.Grpc.DepartmentDetailModel
            {
                Id = departmentId,
                Code = "TD001",
                Name = "Test Department",
                NameInEnglish = "Test Department EN",
                DepartmentTypeName = "Test Type",
                DepartmentTypeNameInEnglish = "Test Type EN",
                IsSuspended = false,
                IsCancelled = false,
                CreatedBy = 1,
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                LastUpdatedBy = 1,
                LastUpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                DepartmentTypeId = 1
            };

            _grpcDepartmentClientMock?
                .GetDepartmentAsync(
                    Arg.Any<HumanResource.Grpc.GetDepartmentRequest>(),
                    Arg.Any<Metadata>(),
                    Arg.Any<DateTime?>(),
                    Arg.Any<CancellationToken>()
                )
                .Returns(callInfo => GrpcClientTestHelpers.CreateAsyncUnaryCall(grpcDepartmentResponse));

            var grpcPatientResponse = new PatientDetailModel
            {
                Id = patientId,
                Code = "BN001",
                Name = "Nguyen Van A",
                Gender = 1,
                Dob = Timestamp.FromDateTime(new DateTime(1990, 1, 1).ToUniversalTime()),
                Email = "patient@example.com",
                PhoneNumber = "84123456789",
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
                Task.FromResult(grpcPatientResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcPatientClientMock?
                .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Act
            var response = await _client.GetAsync($"/{appointmentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAppointmentByIdResponse>();
            result.Should().NotBeNull();
            result.Appointment.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAppointmentById_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var appointmentId = "1";

            // Act
            var response = await _client.GetAsync($"/{appointmentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAppointmentById_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            SetAuthHeader();
            var appointmentId = "9999";

            // Act
            var response = await _client.GetAsync($"/{appointmentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
