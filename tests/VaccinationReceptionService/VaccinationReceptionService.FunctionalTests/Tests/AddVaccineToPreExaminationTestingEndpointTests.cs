using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class AddVaccineToPreExaminationTestingEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionVaccinationId = 1;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;

        public AddVaccineToPreExaminationTestingEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
            SeedData();
        }

        private void SeedData()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existingVaccinations = dbContext.Vaccinations.Where(v => v.ReceptionVaccinationId == TestReceptionVaccinationId);
            dbContext.Vaccinations.RemoveRange(existingVaccinations);

            var existingReceptionVaccinations = dbContext.ReceptionVaccinations.Where(rv => rv.Id == TestReceptionVaccinationId);
            dbContext.ReceptionVaccinations.RemoveRange(existingReceptionVaccinations);

            var existingReceptions = dbContext.Receptions.Where(r => r.Id == TestReceptionId);
            dbContext.Receptions.RemoveRange(existingReceptions);

            dbContext.SaveChanges();

            // Create Reception if not exists
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = 1,
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
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    IsReadyToUse = false,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    //IsConfirmed = false,
                    IsPreExaminationTesting = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task AddVaccineToPreExaminationTesting_WithValidData_ReturnsSuccess()
        {
            // Arrange
            SetupInventoryServiceMock(true); // Vaccine requires pre-examination testing

            // Act
            var response = await _client.PutAsync($"/pre-examination/{TestReceptionVaccinationId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<AddVaccineToPreExaminationTestingResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify that the vaccination was updated in the database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            updatedVaccination.Should().NotBeNull();
            updatedVaccination!.IsPreExaminationTesting.Should().BeTrue();
            updatedVaccination.VaccinationTestDate.Should().NotBeNull();
        }

        [Fact]
        public async Task AddVaccineToPreExaminationTesting_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PutAsync($"/pre-examination/{TestReceptionVaccinationId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddVaccineToPreExaminationTesting_WithNonExistentId_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentId = 9999;
            SetupInventoryServiceMock(true);

            // Act
            var response = await _client.PutAsync($"/pre-examination/{nonExistentId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task AddVaccineToPreExaminationTesting_WithVaccineNotRequiringTesting_ReturnsBadRequest()
        {
            // Arrange
            SetupInventoryServiceMock(false); // Vaccine does not require pre-examination testing

            // Act
            var response = await _client.PutAsync($"/pre-examination/{TestReceptionVaccinationId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task AddVaccineToPreExaminationTesting_WithVaccineAlreadyTaken_ReturnsBadRequest()
        {
            // Arrange
            SetupInventoryServiceMock(true); // Vaccine requires pre-examination testing

            // Create a confirmed vaccination record to simulate that the vaccine has been taken
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existingVaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.ReceptionVaccinationId == TestReceptionVaccinationId);

            if (existingVaccination == null)
            {
                existingVaccination = new Vaccination
                {
                    PatientId = 1,
                    ReceptionVaccinationId = TestReceptionVaccinationId,
                    MedicineBatchId = TestVaccineId,
                    BatchNumber = "BATCH-001",
                    MedicineId = TestVaccineId,
                    MedicineName = "Test Vaccine",
                    VaccinationDate = DateTime.UtcNow.AddDays(-1), // Vaccination was done yesterday
                    DoctorId = 1,
                    IsConfirmed = true, // This is the key field - vaccine is confirmed as taken
                    DoseNumber = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(existingVaccination);
                await dbContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.PutAsync($"/pre-examination/{TestReceptionVaccinationId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        private void SetupInventoryServiceMock(bool isRequiredTestingBeforeUse)
        {
            var medicineInfo = new GetMedicineInformationResponse
            {
                MedicineId = TestVaccineId,
                MedicineName = "Test Vaccine",
                IsRequiredTestingBeforeUse = isRequiredTestingBeforeUse,
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);
        }
    }
}