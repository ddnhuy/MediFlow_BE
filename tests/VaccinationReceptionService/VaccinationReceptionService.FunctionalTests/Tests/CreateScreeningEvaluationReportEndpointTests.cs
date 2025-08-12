using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CreateScreeningEvaluationReportEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;

        public CreateScreeningEvaluationReportEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                dbContext.SaveChanges();
            }
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
                HasAbnormalCry: false,
                HasPaleSkinOrLips: false,
                HasPoorFeeding: false,
                IsPretermBelow34Weeks: false,
                HasImmunodeficiencyOrSuspectedHiv: false,
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
                HasAbnormalCry: false,
                HasPaleSkinOrLips: false,
                HasPoorFeeding: false,
                IsPretermBelow34Weeks: false,
                HasImmunodeficiencyOrSuspectedHiv: false,
                IsEligibleForVaccination: true,
                IsContraindicatedForVaccination: false,
                IsVaccinationDeferred: false,
                IsReferredToHospital: false,
                ReceptionId: TestReceptionId
            );

            _factory.HospitalServiceMock
             .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
             .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
             {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceCode = "EXAMFEE",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    },
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 2,
                        ServiceCode = "IM",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    }
             });
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
                HasAbnormalCry: false,
                HasPaleSkinOrLips: false,
                HasPoorFeeding: false,
                IsPretermBelow34Weeks: false,
                HasImmunodeficiencyOrSuspectedHiv: false,
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

        [Fact]
        public async Task CreateScreeningEvaluationReport_WithInvalidReceptionId_ReturnsBadRequest()
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
                HasAbnormalCry: false,
                HasPaleSkinOrLips: false,
                HasPoorFeeding: false,
                IsPretermBelow34Weeks: false,
                HasImmunodeficiencyOrSuspectedHiv: false,
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
        }
    }
}