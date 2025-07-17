using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Application.Vaccinations.Queries.GetPatientVaccination;
using VaccinationReception.Domain.Models;
using VaccinationReception.Domain.Enums;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetPatientVaccinationEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestReceptionId = 1;
        private const int TestReceptionVaccinationId = 1;
        private const int TestDoctorId = 1;

        public GetPatientVaccinationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Create ScreeningEvaluationReport if not exists
            var screeningReport = dbContext.ScreeningEvaluationReports
                .FirstOrDefault(sr => sr.ReceptionId == TestReceptionId);
            if (screeningReport == null)
            {
                screeningReport = new ScreeningEvaluationReport
                {
                    ReceptionId = TestReceptionId,
                    WeightKg = 65.5,
                    BodyTemperatureC = 36.8,
                    BloodPressureSystolic = 120,
                    BloodPressureDiastolic = 80,
                    IsEligibleForVaccination = true,
                    IsContraindicatedForVaccination = false,
                    IsVaccinationDeferred = false,
                    IsReferredToHospital = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ScreeningEvaluationReports.Add(screeningReport);
            }

            // Create ReceptionVaccination with Paid status and unconfirmed if not exists
            var receptionVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId,
                    ReceptionId = TestReceptionId,
                    VaccineId = 1,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.Paid, 
                    //IsConfirmed = false, 
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

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetPatientVaccination_WithValidData_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/waiting-patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientVaccinationQueryResult>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPatientVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/vaccination/waiting-patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPatientVaccination_WithNoEligibleData_ReturnsEmptyList()
        {
            // Arrange
            // Update the ReceptionVaccination to be confirmed (making it ineligible)
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var receptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            if (receptionVaccination != null)
            {
                //receptionVaccination.IsConfirmed = true;
                await dbContext.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync("/vaccination/waiting-patients");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetPatientVaccinationQueryResult>();
            result.Should().NotBeNull();
            result!.PatientVaccinationItems.Should().NotBeNull();
            result.PatientVaccinationItems.Should().BeEmpty();

            // Clean up - restore the original state
            if (receptionVaccination != null)
            {
                //receptionVaccination.IsConfirmed = false;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}