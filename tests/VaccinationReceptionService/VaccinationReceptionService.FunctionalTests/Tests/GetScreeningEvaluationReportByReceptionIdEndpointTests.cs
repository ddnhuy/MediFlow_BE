using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetScreeningEvaluationReportByReceptionIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestPatientId = 1;
        private const int TestServiceTypeId = 1;
        private const int TestScreeningReportId = 1;

        public GetScreeningEvaluationReportByReceptionIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedData();
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create ServiceType if not exists
            var serviceType = dbContext.ServiceTypes.FirstOrDefault(st => st.Id == TestServiceTypeId);
            if (serviceType == null)
            {
                serviceType = new ServiceType
                {
                    Id = TestServiceTypeId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ServiceTypes.Add(serviceType);
            }

            // Create Reception if not exists
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = TestServiceTypeId,
                    PatientId = TestPatientId,
                    ReceptionDate = DateTime.UtcNow,
                    IsCancelled = false,
                    IsSuspended = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Create ScreeningEvaluationReport
            var existingReport = dbContext.ScreeningEvaluationReports
                .FirstOrDefault(ser => ser.ReceptionId == TestReceptionId);
            if (existingReport == null)
            {
                var screeningReport = new ScreeningEvaluationReport
                {
                    Id = TestScreeningReportId,
                    ReceptionId = TestReceptionId,
                    ParentFullName = "Parent Name",
                    ParentPhoneNumber = "0123456789",
                    WeightKg = 65.5,
                    BodyTemperatureC = 36.5,
                    BloodPressureSystolic = 120,
                    BloodPressureDiastolic = 80,
                    HasSevereFeverAfterPreviousVaccination = false,
                    HasAcuteOrChronicDisease = false,
                    IsOnOrRecentlyEndedCorticosteroids = false,
                    HasAbnormalTemperatureOrVitals = false,
                    HasAbnormalHeartSound = false,
                    HasHeartValveDisorder = false,
                    HasNeurologicalAbnormalities = false,
                    IsUnderweightBelow2000g = false,
                    HasOtherContraindications = false,
                    HasAbnormalCry = false,
                    HasPaleSkinOrLips = false,
                    HasPoorFeeding = false,
                    IsPretermBelow34Weeks = false,
                    HasImmunodeficiencyOrSuspectedHiv = false,
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

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithValidReceptionId_ReturnsOkWithReport()
        {
            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetScreeningEvaluationReportByReceptionIdResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Report.Should().NotBeNull();
            result.Report.Id.Should().Be(TestScreeningReportId);
            result.Report.ReceptionId.Should().Be(TestReceptionId);
            result.Report.ParentFullName.Should().Be("Parent Name");
            result.Report.ParentPhoneNumber.Should().Be("0123456789");
            result.Report.WeightKg.Should().Be(65.5);
            result.Report.BodyTemperatureC.Should().Be(36.5);
            result.Report.BloodPressureSystolic.Should().Be(120);
            result.Report.BloodPressureDiastolic.Should().Be(80);
            result.Report.IsEligibleForVaccination.Should().BeTrue();
            result.Report.IsContraindicatedForVaccination.Should().BeFalse();
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithNonExistentReceptionId_ReturnsNotFound()
        {
            // Arrange
            const int nonExistentReceptionId = 999;

            // Act
            var response = await _client.GetAsync($"/receptions/{nonExistentReceptionId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithInvalidReceptionId_ReturnsBadRequest()
        {
            // Arrange
            const int invalidReceptionId = 0;

            // Act
            var response = await _client.GetAsync($"/receptions/{invalidReceptionId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithNegativeReceptionId_ReturnsBadRequest()
        {
            // Arrange
            const int negativeReceptionId = -1;

            // Act
            var response = await _client.GetAsync($"/receptions/{negativeReceptionId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithUnauthorizedRequest_ReturnsUnauthorized()
        {
            // Arrange - Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithReceptionButNoReport_ReturnsOkWithNullReport()
        {
            // Arrange - Create a reception without screening report
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            const int receptionWithoutReportId = 2;
            var receptionWithoutReport = new Reception
            {
                Id = receptionWithoutReportId,
                ServiceTypeId = TestServiceTypeId,
                PatientId = TestPatientId,
                ReceptionDate = DateTime.UtcNow,
                IsCancelled = false,
                IsSuspended = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Receptions.Add(receptionWithoutReport);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/receptions/{receptionWithoutReportId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetScreeningEvaluationReportByReceptionIdResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Report.Should().BeNull();
        }

        [Fact]
        public async Task GetScreeningEvaluationReportByReceptionId_WithCancelledReception_ReturnsNotFound()
        {
            // Arrange - Create a cancelled reception
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            const int cancelledReceptionId = 3;
            var cancelledReception = new Reception
            {
                Id = cancelledReceptionId,
                ServiceTypeId = TestServiceTypeId,
                PatientId = TestPatientId,
                ReceptionDate = DateTime.UtcNow,
                IsCancelled = true, // Cancelled reception
                IsSuspended = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.Receptions.Add(cancelledReception);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/receptions/{cancelledReceptionId}/screening-evaluation-report");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
