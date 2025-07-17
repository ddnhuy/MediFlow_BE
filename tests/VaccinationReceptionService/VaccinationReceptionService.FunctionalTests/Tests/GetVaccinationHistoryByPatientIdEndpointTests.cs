using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetVaccinationHistoryByPatientIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionId = 1;
        private const int TestReceptionVaccinationId = 1;
        private const int TestVaccinationId = 1;
        private const int TestMedicineId = 1;
        private const int TestDoctorId = 1;

        public GetVaccinationHistoryByPatientIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            ConfigureMocks();
            SeedData();
        }

        private void ConfigureMocks()
        {
            // Setup Patient gRPC mock
            var patientResponse = new PatientDetailModel
            {
                Id = TestPatientId,
                Code = "PAT-001",
                Name = "John Doe",
                Gender = 1, // Male
                PhoneNumber = "123456789",
                AddressDetail = "123 Main St",
                Ward = "Central Ward",
                District = "Downtown",
                Province = "Test Province",
                Dob = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-30))
            };

            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(patientResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock?
                .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Setup Medicine Information mock
            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = TestMedicineId,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "mRNA",
                MedicineTypeName = "Vaccine",
                Concentration = "30 mg/mL",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1 };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // Setup ApplicationUser gRPC mock for doctor information
            var doctorResponse = new ApplicationUserDetailModel
            {
                Id = TestDoctorId,
                Name = "Dr. Smith"
            };

            var doctorAsyncUnaryCall = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Metadata>(), null, default)
                .Returns(doctorAsyncUnaryCall);
        }

        private void SeedData()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create Reception if not exists
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = TestPatientId,
                    ReceptionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Create ReceptionVaccination if not exists
            var receptionVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId,
                    ReceptionId = TestReceptionId,
                    VaccineId = TestMedicineId,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    //IsConfirmed = false,
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    UnitPrice = 100.00m,
                    DoctorId = TestDoctorId,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            // Create Vaccination if not exists
            var vaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.Id == TestVaccinationId);
            if (vaccination == null)
            {
                vaccination = new Vaccination
                {
                    Id = TestVaccinationId,
                    PatientId = TestPatientId,
                    ReceptionVaccinationId = TestReceptionVaccinationId,
                    MedicineBatchId = 1,
                    BatchNumber = "BATCH-001",
                    MedicineId = TestMedicineId,
                    MedicineName = "COVID-19 Vaccine",
                    VaccinationDate = DateTime.UtcNow,
                    Note = "Test vaccination",
                    DoctorId = TestDoctorId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(vaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetVaccinationHistory_WithValidData_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.PatientName.Should().Be("John Doe");
            result.Gender.Should().Be("Nam");
            result.PatientCode.Should().Be("PAT-001");

            result.VaccinationHistoryItems.Should().NotBeNull();
            result.VaccinationHistoryItems.Should().HaveCount(1);

            var historyItem = result.VaccinationHistoryItems.First();
            historyItem.MedicineName.Should().Be("COVID-19 Vaccine");
            historyItem.MedicineTypeName.Should().Be("mRNA");
            historyItem.DoctorName.Should().Be("B.S Dr. Smith");
        }

        [Fact]
        public async Task GetVaccinationHistory_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/vaccination/patient/{TestPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetVaccinationHistory_WithNonExistentPatientId_ReturnsEmptyHistory()
        {
            // Act
            var nonExistentPatientId = 9999;
            var response = await _client.GetAsync($"/vaccination/patient/{nonExistentPatientId}/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetVaccinationHistoryByPatientIdResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistoryItems.Should().NotBeNull();
            result.VaccinationHistoryItems.Should().BeEmpty();
        }
    }
}