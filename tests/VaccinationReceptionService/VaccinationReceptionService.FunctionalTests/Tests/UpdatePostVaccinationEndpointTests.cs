using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.Vaccinations.Commands.UpdatePostVaccination;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdatePostVaccinationEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionVaccinationId = 1;
        private const int TestVaccinationId = 1;
        private const int TestDoctorId = 1;

        public UpdatePostVaccinationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == 1);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = 1,
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
                    ReceptionId = 1,
                    VaccineId = 1,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    IsConfirmed = true,
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
                    MedicineId = 1,
                    MedicineName = "Test Vaccine",
                    VaccinationDate = DateTime.UtcNow,
                    Note = "Test vaccination",
                    DoctorId = TestDoctorId,
                    // PostVaccination properties - initially false/null
                    ObservationConfirmed = false,
                    HasReaction = false,
                    ReactionDate = null,
                    PostVaccinationResult = null,
                    PostVaccinationDate = null,
                    HasFeverAbove39 = false,
                    HasInjectionSiteReaction = false,
                    HasOtherReaction = false,
                    OtherReactionDescription = null,
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
        public async Task UpdatePostVaccination_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var command = new UpdatePostVaccinationCommand(
                Id: TestVaccinationId,
                ObservationConfirmed: true,
                HasReaction: false,
                ReactionDate: null,
                PostVaccinationResult: "Patient tolerated vaccination well",
                PostVaccinationDate: DateTime.UtcNow,
                HasFeverAbove39: false,
                HasInjectionSiteReaction: false,
                HasOtherReaction: false,
                OtherReactionDescription: null
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{TestVaccinationId}/post-vaccination", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdatePostVaccinationResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify that the vaccination was updated in the database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedVaccination = await dbContext.Vaccinations
                .FirstOrDefaultAsync(v => v.Id == TestVaccinationId);

            updatedVaccination.Should().NotBeNull();
            updatedVaccination!.ObservationConfirmed.Should().BeTrue();
            updatedVaccination.HasReaction.Should().BeFalse();
            updatedVaccination.PostVaccinationResult.Should().Be("Patient tolerated vaccination well");
            updatedVaccination.PostVaccinationDate.Should().NotBeNull();
            updatedVaccination.HasFeverAbove39.Should().BeFalse();
            updatedVaccination.HasInjectionSiteReaction.Should().BeFalse();
            updatedVaccination.HasOtherReaction.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePostVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new UpdatePostVaccinationCommand(
                Id: TestVaccinationId,
                ObservationConfirmed: true,
                HasReaction: false,
                ReactionDate: null,
                PostVaccinationResult: "Test result",
                PostVaccinationDate: DateTime.UtcNow,
                HasFeverAbove39: false,
                HasInjectionSiteReaction: false,
                HasOtherReaction: false,
                OtherReactionDescription: null
            );
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{TestVaccinationId}/post-vaccination", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdatePostVaccination_WithNonExistentVaccination_ReturnsBadRequest()
        {
            // Arrange
            var nonExistentVaccinationId = 9999;
            var command = new UpdatePostVaccinationCommand(
                Id: nonExistentVaccinationId,
                ObservationConfirmed: true,
                HasReaction: true,
                ReactionDate: DateTime.UtcNow,
                PostVaccinationResult: "Patient had adverse reaction",
                PostVaccinationDate: DateTime.UtcNow,
                HasFeverAbove39: true,
                HasInjectionSiteReaction: true,
                HasOtherReaction: false,
                OtherReactionDescription: null
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/vaccination/{nonExistentVaccinationId}/post-vaccination", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }
    }
}