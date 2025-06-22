using Testcontainers.RabbitMq;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CreateReceptionVaccinationEndpointTests : CreateReceptionVaccinationBaseTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly CreateReceptionVaccinationFunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private const int TestDoctorId = 1;
        private const int TestServiceTypeId = 1;

        public CreateReceptionVaccinationEndpointTests(CreateReceptionVaccinationFunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        public async Task InitializeAsync()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create ServiceType if not exists
            var serviceType = await dbContext.ServiceTypes.FirstOrDefaultAsync(st => st.Id == TestServiceTypeId);
            if (serviceType == null)
            {
                serviceType = new ServiceType
                {
                    Id = TestServiceTypeId,
                    Name = "Test Service Type",
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                await dbContext.ServiceTypes.AddAsync(serviceType);
            }

            // Create Reception if not exists
            var reception = await dbContext.Receptions.FirstOrDefaultAsync(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = TestServiceTypeId,
                    PatientId = 1, // Thêm PatientId
                    ReceptionDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.Receptions.AddAsync(reception);
            }

            await dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: TestReceptionId,
                VaccineId: TestVaccineId,
                Quantity: 1,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow,
                InvoiceDate: DateTime.UtcNow,
                AppointmentDate: DateTime.UtcNow,
                IsPaid: false,
                IsConfirmed: false,
                Note: null,
                TestResultEntry: null,
                DoctorId: TestDoctorId
            );
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithValidData_ReturnsCreated()
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
                Email = "abcd@example.com",
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

            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: TestReceptionId,
                VaccineId: TestVaccineId,
                Quantity: 1,
                IsReadyToUse: true,
                ScheduledDate: DateTime.Now.AddDays(7),
                InvoiceDate: DateTime.Now,
                AppointmentDate: DateTime.Now.AddDays(7),
                IsPaid: false,
                IsConfirmed: false,
                Note: null,
                TestResultEntry: null,
                DoctorId: TestDoctorId
            );

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreateReceptionVaccinationResponse>();
            result.Should().NotBeNull();
            result!.ReceptionVaccinationId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: 0,
                VaccineId: 0,
                Quantity: 0,
                IsReadyToUse: true,
                ScheduledDate: DateTime.Now,
                InvoiceDate: DateTime.Now,
                AppointmentDate: DateTime.Now,
                IsPaid: false,
                IsConfirmed: false,
                Note: null,
                TestResultEntry: null,
                DoctorId: 0
            );

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithInvalidData_ReturnsNotFound()
        {
            // Arrange
            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: 9999,
                VaccineId: TestVaccineId,
                Quantity: 1,
                IsReadyToUse: true,
                ScheduledDate: DateTime.Now.AddDays(7),
                InvoiceDate: DateTime.Now,
                AppointmentDate: DateTime.Now.AddDays(7),
                IsPaid: false,
                IsConfirmed: false,
                Note: null,
                TestResultEntry: null,
                DoctorId: TestDoctorId
            );

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}