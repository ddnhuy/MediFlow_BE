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
    public class CreateScreeningEvaluationReportEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;

        public CreateScreeningEvaluationReportEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                await dbContext.SaveChangesAsync();
            }
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CreateScreeningEvaluationReport_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new CreateScreeningEvaluationReportCommand(
                ParentFullName: "Test Parent",
                ParentPhoneNumber: "0123456789",
                WeightKg: 70.5,
                BodyTemperatureC: 37.0,
                BloodPressureSystolic: 120,
                BloodPressureDiastolic: 80,
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
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PostAsJsonAsync("/screening-evaluations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateScreeningEvaluationReport_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new CreateScreeningEvaluationReportCommand(
                ParentFullName: "Test Parent",
                ParentPhoneNumber: "0123456789",
                WeightKg: 70.5,
                BodyTemperatureC: 37.0,
                BloodPressureSystolic: 120,
                BloodPressureDiastolic: 80,
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

            // Act
            var response = await _client.PostAsJsonAsync("/screening-evaluations", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreateScreeningEvaluationResponse>();
            result.Should().NotBeNull();
            result!.screeningEvaluationId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateScreeningEvaluationReport_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateScreeningEvaluationReportCommand(
                ParentFullName: "",
                ParentPhoneNumber: "123",
                WeightKg: -1,
                BodyTemperatureC: 45.0,
                BloodPressureSystolic: 0,
                BloodPressureDiastolic: 0,
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
                ReceptionId: 0
            );

            // Act
            var response = await _client.PostAsJsonAsync("/screening-evaluations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }
    }
}