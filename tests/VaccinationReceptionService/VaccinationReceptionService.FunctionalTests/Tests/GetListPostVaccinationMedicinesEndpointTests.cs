using VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationMedicines;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetListPostVaccinationMedicinesEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccinationId = 1;
        private const int TestReceptionVaccinationId = 1;

        public GetListPostVaccinationMedicinesEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                    VaccineId = 1,
                    Quantity = 2,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatusForItem.Paid,
                    //IsConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1,
                    RequestNumber = "REQ-001"
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            // Create Vaccinations if not exists
            var vaccination = dbContext.Vaccinations.FirstOrDefault(v => v.Id == TestVaccinationId);
            if (vaccination == null)
            {
                vaccination = new Vaccination
                {
                    Id = TestVaccinationId,
                    PatientId = 1,
                    ReceptionVaccinationId = TestReceptionVaccinationId,
                    MedicineBatchId = 1,
                    BatchNumber = "BATCH001",
                    MedicineId = 1,
                    MedicineName = "COVID-19 Vaccine",
                    VaccinationDate = DateTime.UtcNow.AddDays(-1),
                    DoctorId = 1,
                    ObservationConfirmed = false, // This should be false to be included in results
                    HasReaction = false,
                    ReactionDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(vaccination);
            }

            // Create additional vaccination with observation confirmed (should not be included in results)
            var confirmedVaccination = dbContext.Vaccinations.FirstOrDefault(v => v.Id == 2);
            if (confirmedVaccination == null)
            {
                confirmedVaccination = new Vaccination
                {
                    Id = 2,
                    PatientId = 1,
                    ReceptionVaccinationId = TestReceptionVaccinationId,
                    MedicineBatchId = 2,
                    BatchNumber = "BATCH002",
                    MedicineId = 2,
                    MedicineName = "Flu Vaccine",
                    VaccinationDate = DateTime.UtcNow.AddDays(-2),
                    DoctorId = 1,
                    ObservationConfirmed = true, // This should be true to be excluded from results
                    HasReaction = false,
                    ReactionDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(confirmedVaccination);
            }

            // Create vaccination for different reception (should not be included in results)
            var differentReceptionVaccination = dbContext.Vaccinations.FirstOrDefault(v => v.Id == 3);
            if (differentReceptionVaccination == null)
            {
                // First create a different reception
                var differentReception = dbContext.Receptions.FirstOrDefault(r => r.Id == 999);
                if (differentReception == null)
                {
                    differentReception = new Reception
                    {
                        Id = 999,
                        ServiceTypeId = 1,
                        PatientId = 2,
                        ReceptionDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1
                    };
                    dbContext.Receptions.Add(differentReception);
                }

                // Create reception vaccination for different reception
                var differentReceptionVacc = dbContext.ReceptionVaccinations.FirstOrDefault(rv => rv.Id == 999);
                if (differentReceptionVacc == null)
                {
                    differentReceptionVacc = new ReceptionVaccination
                    {
                        Id = 999,
                        ReceptionId = 999,
                        VaccineId = 1,
                        Quantity = 1,
                        IsReadyToUse = true,
                        ScheduledDate = DateTime.UtcNow,
                        InvoiceDate = DateTime.UtcNow,
                        AppointmentDate = DateTime.UtcNow,
                        PaymentStatus = PaymentStatusForItem.Paid,
                        //IsConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = 1,
                        RequestNumber = "REQ-999"
                    };
                    dbContext.ReceptionVaccinations.Add(differentReceptionVacc);
                }

                differentReceptionVaccination = new Vaccination
                {
                    Id = 3,
                    PatientId = 2,
                    ReceptionVaccinationId = 999,
                    MedicineBatchId = 3,
                    BatchNumber = "BATCH003",
                    MedicineId = 3,
                    MedicineName = "Hepatitis B Vaccine",
                    VaccinationDate = DateTime.UtcNow.AddDays(-3),
                    DoctorId = 1,
                    ObservationConfirmed = false,
                    HasReaction = false,
                    ReactionDate = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(differentReceptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetListPostVaccinationMedicines_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/vaccination/post-vaccination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetListPostVaccinationMedicines_WithValidReceptionId_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync($"/vaccination/post-vaccination/reception/{TestReceptionId}/medicines");

            // Debug log
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<GetListPostVaccinationMedicinesResult>>();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            // Should only return vaccinations that are not observation confirmed
            result.Should().HaveCount(1);
            var vaccination = result.First();
            vaccination.VaccinationId.Should().Be(TestVaccinationId);
            vaccination.MedicineName.Should().Be("COVID-19 Vaccine");
            vaccination.Quantity.Should().Be(2);
            vaccination.ObservationConfirmed.Should().BeFalse();
            vaccination.ReactionDate.Should().BeNull();
        }

        [Fact]
        public async Task GetListPostVaccinationMedicines_WithInvalidReceptionId_ReturnsEmptyList()
        {
            // Arrange
            var invalidReceptionId = 99999;

            // Act
            var response = await _client.GetAsync($"/vaccination/post-vaccination/reception/{invalidReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<GetListPostVaccinationMedicinesResult>>();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetListPostVaccinationMedicines_ReturnsCorrectDataStructure()
        {
            // Act
            var response = await _client.GetAsync($"/vaccination/post-vaccination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<GetListPostVaccinationMedicinesResult>>();
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            var vaccination = result.First();
            vaccination.Should().NotBeNull();
            vaccination.VaccinationId.Should().BeGreaterThan(0);
            vaccination.MedicineName.Should().NotBeNullOrEmpty();
            vaccination.Quantity.Should().BeGreaterThan(0);
            vaccination.VaccinationDate.Should().NotBe(DateTime.MinValue);
            vaccination.ObservationConfirmed.Should().BeFalse(); // Only unconfirmed observations should be returned
        }

        [Fact]
        public async Task GetListPostVaccinationMedicines_WithAllVaccinationsConfirmed_ReturnsEmptyList()
        {
            // Arrange - Update all vaccinations to be observation confirmed
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var vaccinations = dbContext.Vaccinations
                .Where(v => v.ReceptionVaccination!.ReceptionId == TestReceptionId)
                .ToList();

            foreach (var vaccination in vaccinations)
            {
                vaccination.ObservationConfirmed = true;
            }
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/vaccination/post-vaccination/reception/{TestReceptionId}/medicines");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<List<GetListPostVaccinationMedicinesResult>>();
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            // Clean up - Reset the data for other tests
            foreach (var vaccination in vaccinations)
            {
                if (vaccination.Id == TestVaccinationId)
                {
                    vaccination.ObservationConfirmed = false;
                }
            }
            dbContext.SaveChanges();
        }
    }
}