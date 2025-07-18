using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationStatus;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdateVaccinationStatusEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionVaccinationId = 1;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private int _testVaccinationId;

        public UpdateVaccinationStatusEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            // Create Vaccination if not exists
            var vaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.ReceptionVaccinationId == TestReceptionVaccinationId);
            if (vaccination == null)
            {
                vaccination = new Vaccination
                {
                    ReceptionVaccinationId = TestReceptionVaccinationId,
                    PatientId = 1,
                    MedicineBatchId = TestVaccineId,
                    BatchNumber = "BATCH-001",
                    MedicineId = TestVaccineId,
                    MedicineName = "Test Vaccine",
                    Note = "Initial",
                    DoctorId = 1,
                    VaccinationDate = DateTime.UtcNow,
                    IsConfirmed = false,
                    DoseNumber = 1
                };
                dbContext.Vaccinations.Add(vaccination);
                dbContext.SaveChanges();
            }
            _testVaccinationId = vaccination.Id;
        }

        [Fact]
        public async Task UpdateVaccinationStatus_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var command = new UpdateVaccinationStatusCommand(
                VaccinationId: _testVaccinationId,
                Status: true
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{_testVaccinationId}/status", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateVaccinationStatusCommandResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify that the vaccination status was updated in the database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedVaccination = await dbContext.Vaccinations
                .FirstOrDefaultAsync(v => v.Id == _testVaccinationId);

            updatedVaccination.Should().NotBeNull();
            updatedVaccination!.IsConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateVaccinationStatus_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new UpdateVaccinationStatusCommand(
                VaccinationId: _testVaccinationId,
                Status: true
            );
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{_testVaccinationId}/status", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateVaccinationStatus_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var mismatchedId = _testVaccinationId + 1;
            var command = new UpdateVaccinationStatusCommand(
                VaccinationId: _testVaccinationId,
                Status: true
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{mismatchedId}/status", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateVaccinationStatus_WithNonExistentId_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentId = 9999;
            var command = new UpdateVaccinationStatusCommand(
                VaccinationId: nonExistentId,
                Status: true
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{nonExistentId}/status", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }
    }
}