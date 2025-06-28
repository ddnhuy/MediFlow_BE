using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Pagination;
using HumanResource.Grpc;
using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetMedicineListForPreExaminationEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestPatientId = 1;
        private const int TestVaccineId = 1;
        private const int TestDoctorId = 1;
        private const int TestReceptionVaccinationId = 1;

        public GetMedicineListForPreExaminationEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            ConfigureMocks();
            SeedData();
        }

        private void ConfigureMocks()
        {
            // Arrange - Create mock response for IPatientGrpcClient
            var patientsResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = 1,
                    Code = "BN100",
                    Name = "Test Patient 1",
                    IdentityCard = "01233434",
                    PhoneNumber = "0123456789",
                    DOB = new DateTime(1990, 1, 1),
                    Gender = 1 // Male
                },
                new PatientSummaryDTO
                {
                    Id = 2,
                    Code = "BN101",
                    Name = "Test Patient 2",
                    IdentityCard = "01223456",
                    PhoneNumber = "0123456789",
                    DOB = new DateTime(1995, 1, 1),
                    Gender = 0 // Female
                }
            };

            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(patientsResponse);

            // Medicine Information mock
            var medicineInfo = new GetMedicineInformationResponse
            {
                MedicineId = TestVaccineId,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "mRNA",
                MedicineTypeName = "Vaccine",
                Concentration = "30 mg/mL",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // ApplicationUser gRPC mock for doctor information
            var doctorResponse = new ApplicationUserDetailModel
            {
                Id = TestDoctorId,
                Name = "Dr. Smith"
            };

            var doctorAsyncUnaryCall = new Grpc.Core.AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse),
                Task.FromResult(new Grpc.Core.Metadata()),
                () => Grpc.Core.Status.DefaultSuccess,
                () => new Grpc.Core.Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Grpc.Core.Metadata>(), null, default)
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

            // Create ReceptionVaccination with pre-examination testing enabled
            var receptionVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId,
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    IsConfirmed = false,
                    IsPreExaminationTesting = true, // This is the key field for this query
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-1),
                    TestResultEntry = "Normal",
                    DoctorId = TestDoctorId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    UnitPrice = 100.00m,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetMedicineListForPreExamination_WithValidData_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync($"/pre-examination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetMedicineListForPreExaminationResponse>();

            result.Should().NotBeNull();
            result!.PreExaminationMedicineItems.Should().NotBeNull();
            result.PreExaminationMedicineItems.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetMedicineListForPreExamination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/pre-examination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }      

        [Fact]
        public async Task GetMedicineListForPreExamination_WithInvalidReceptionId_ReturnsBadRequest()
        {
            // Arrange
            var invalidReceptionId = 0; // Invalid reception ID

            // Act
            var response = await _client.GetAsync($"/pre-examination/reception/{invalidReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}