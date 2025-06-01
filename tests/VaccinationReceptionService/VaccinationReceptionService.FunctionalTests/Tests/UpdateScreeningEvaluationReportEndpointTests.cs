using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdateScreeningEvaluationReportEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReportId = 1;
        private const int TestReceptionId = 1;

        public UpdateScreeningEvaluationReportEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Create Reception if not exists
            var reception = await dbContext.Receptions.FirstOrDefaultAsync(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = 1,
                    ReceptionDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.Receptions.AddAsync(reception);
            }

            // Create ScreeningEvaluationReport if not exists
            var report = await dbContext.ScreeningEvaluationReports
                .FirstOrDefaultAsync(r => r.Id == TestReportId);
            if (report == null)
            {
                report = new ScreeningEvaluationReport
                {
                    Id = TestReportId,
                    ReceptionId = TestReceptionId,
                    ParentFullName = "Test Parent",
                    ParentPhoneNumber = "0123456789",
                    WeightKg = 10.5,
                    BodyTemperatureC = 37.0,
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
                    IsEligibleForVaccination = true,
                    IsContraindicatedForVaccination = false,
                    IsVaccinationDeferred = false,
                    IsReferredToHospital = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.ScreeningEvaluationReports.AddAsync(report);
            }

            await dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task UpdateScreeningEvaluationReport_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = CreateValidCommand();

            // Act
            var response = await _client.PutAsJsonAsync($"/screeningevaluation/{TestReportId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateScreeningEvaluationReport_WithValidData_ReturnsOk()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var response = await _client.PutAsJsonAsync($"/screeningevaluation/{TestReportId}", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateScreeningEvaluationReportResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify data was updated in database
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var report = await dbContext.ScreeningEvaluationReports
                .FirstOrDefaultAsync(r => r.Id == TestReportId);

            report.Should().NotBeNull();
            report!.ParentFullName.Should().Be(command.ParentFullName);
            report.ParentPhoneNumber.Should().Be(command.ParentPhoneNumber);
            report.WeightKg.Should().Be(command.WeightKg);
            report.BodyTemperatureC.Should().Be(command.BodyTemperatureC);
            report.BloodPressureSystolic.Should().Be(command.BloodPressureSystolic);
            report.BloodPressureDiastolic.Should().Be(command.BloodPressureDiastolic);
            report.HasSevereFeverAfterPreviousVaccination.Should().Be(command.HasSevereFeverAfterPreviousVaccination);
            report.HasAcuteOrChronicDisease.Should().Be(command.HasAcuteOrChronicDisease);
            report.IsOnOrRecentlyEndedCorticosteroids.Should().Be(command.IsOnOrRecentlyEndedCorticosteroids);
            report.HasAbnormalTemperatureOrVitals.Should().Be(command.HasAbnormalTemperatureOrVitals);
            report.HasAbnormalHeartSound.Should().Be(command.HasAbnormalHeartSound);
            report.HasHeartValveDisorder.Should().Be(command.HasHeartValveDisorder);
            report.HasNeurologicalAbnormalities.Should().Be(command.HasNeurologicalAbnormalities);
            report.IsUnderweightBelow2000g.Should().Be(command.IsUnderweightBelow2000g);
            report.HasOtherContraindications.Should().Be(command.HasOtherContraindications);
            report.IsEligibleForVaccination.Should().Be(command.IsEligibleForVaccination);
            report.IsContraindicatedForVaccination.Should().Be(command.IsContraindicatedForVaccination);
            report.IsVaccinationDeferred.Should().Be(command.IsVaccinationDeferred);
            report.IsReferredToHospital.Should().Be(command.IsReferredToHospital);
        }

        [Fact]
        public async Task UpdateScreeningEvaluationReport_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var command = CreateValidCommand();
            var differentId = TestReportId + 1;

            // Act
            var response = await _client.PutAsJsonAsync($"/screeningevaluation/{differentId}", command);

            var content = await response.Content.ReadAsStringAsync();
           
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            content.Should().NotBeNull();
            content.Should().Contain("ID trong đường dẫn không khớp với ID trong nội dung yêu cầu");
        }

        private UpdateScreeningEvaluationReportCommand CreateValidCommand()
        {
            return new UpdateScreeningEvaluationReportCommand(
                Id: TestReportId,
                ParentFullName: "Updated Parent Name",
                ParentPhoneNumber: "0987654321",
                WeightKg: 11.5,
                BodyTemperatureC: 36.8,
                BloodPressureSystolic: 118,
                BloodPressureDiastolic: 78,
                HasSevereFeverAfterPreviousVaccination: false,
                HasAcuteOrChronicDisease: false,
                IsOnOrRecentlyEndedCorticosteroids: false,
                HasAbnormalTemperatureOrVitals: false,
                HasAbnormalHeartSound: false,
                HasHeartValveDisorder: false,
                HasNeurologicalAbnormalities: false,
                IsUnderweightBelow2000g: false,
                HasOtherContraindications: false,
                IsEligibleForVaccination: true,
                IsContraindicatedForVaccination: false,
                IsVaccinationDeferred: false,
                IsReferredToHospital: false,
                ReceptionId: TestReceptionId
            );
        }
    }
}