using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.Vaccinations.Commands.CreateVaccination;
using VaccinationReception.Domain.Models;
using CreateVaccinationResponse = VaccinationReception.Application.Vaccinations.Commands.CreateVaccination.CreateVaccinationResponse;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CreateVaccinationEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionVaccinationId = 1;
        private const int TestMedicineId = 1;
        private const int TestDoctorId = 1;
        private const int TestReceptionId = 1;

        public CreateVaccinationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.NotPaid,
                    IsConfirmed = false,
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
        public async Task CreateVaccination_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var command = new CreateVaccinationCommand(
                PatientId: TestPatientId,
                ReceptionVaccinationId: TestReceptionVaccinationId,
                MedicineBatchId: TestMedicineId,
                BatchNumber: "BATCH-001",
                MedicineId: TestMedicineId,
                MedicineName: "Test Vaccine",
                Note: "Test vaccination note",
                DoctorId: TestDoctorId
            );

            // Act
            var response = await _client.PostAsJsonAsync("/vaccination", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<CreateVaccinationResponse>();
            result.Should().NotBeNull();
            result!.VaccinationId.Should().BeGreaterThan(0);

            // Verify that the vaccination was created in the database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var createdVaccination = await dbContext.Vaccinations
                .FirstOrDefaultAsync(v => v.Id == result.VaccinationId);

            createdVaccination.Should().NotBeNull();
            createdVaccination!.PatientId.Should().Be(TestPatientId);
            createdVaccination.ReceptionVaccinationId.Should().Be(TestReceptionVaccinationId);
            createdVaccination.MedicineId.Should().Be(TestMedicineId);
            createdVaccination.MedicineName.Should().Be("Test Vaccine");
            createdVaccination.DoctorId.Should().Be(TestDoctorId);
            createdVaccination.VaccinationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            // Verify that the reception vaccination was marked as confirmed
            var updatedReceptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);
            updatedReceptionVaccination.Should().NotBeNull();
            updatedReceptionVaccination!.IsConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task CreateVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new CreateVaccinationCommand(
                PatientId: TestPatientId,
                ReceptionVaccinationId: TestReceptionVaccinationId,
                MedicineBatchId: TestMedicineId,
                BatchNumber: "BATCH-001",
                MedicineId: TestMedicineId,
                MedicineName: "Test Vaccine",
                Note: "Test vaccination note",
                DoctorId: TestDoctorId
            );
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PostAsJsonAsync("/vaccination", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }       

        [Fact]
        public async Task CreateVaccination_WithNonExistentReceptionVaccination_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateVaccinationCommand(
                PatientId: TestPatientId,
                ReceptionVaccinationId: 9999, // Non-existent ReceptionVaccinationId
                MedicineBatchId: TestMedicineId,
                BatchNumber: "BATCH-001",
                MedicineId: TestMedicineId,
                MedicineName: "Test Vaccine",
                Note: "Test vaccination note",
                DoctorId: TestDoctorId
            );

            // Act
            var response = await _client.PostAsJsonAsync("/vaccination", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }         
    }
}