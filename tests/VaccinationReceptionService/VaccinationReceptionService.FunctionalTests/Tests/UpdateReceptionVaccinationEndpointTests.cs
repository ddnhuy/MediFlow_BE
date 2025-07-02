using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class UpdateReceptionVaccinationEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionVaccinationId = 1;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;

        public UpdateReceptionVaccinationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                    IsConfirmed = false,
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
        public async Task UpdateReceptionVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = CreateValidCommand();

            // Act
            var response = await _client.PutAsJsonAsync($"/reception-vaccinations/{TestReceptionVaccinationId}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithValidData_ReturnsOk()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var response = await _client.PutAsJsonAsync($"/reception-vaccinations/{TestReceptionVaccinationId}", command);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateReceptionVaccinationResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var receptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == TestReceptionVaccinationId);

            receptionVaccination.Should().NotBeNull();
            receptionVaccination!.Quantity.Should().Be(command.Quantity);
            receptionVaccination.IsReadyToUse.Should().Be(command.IsReadyToUse);

            receptionVaccination.ScheduledDate.Should().BeCloseTo(command.ScheduledDate.Value, TimeSpan.FromSeconds(1));
            receptionVaccination.AppointmentDate.Should().BeCloseTo(command.AppointmentDate, TimeSpan.FromSeconds(1));

            receptionVaccination.Note.Should().Be(command.Note);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var command = CreateValidCommand();
            var differentId = TestReceptionVaccinationId + 1;

            // Act
            var response = await _client.PutAsJsonAsync($"/reception-vaccinations/{differentId}", command);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private UpdateReceptionVaccinationCommand CreateValidCommand()
        {
            return new UpdateReceptionVaccinationCommand(
                Id: TestReceptionVaccinationId,
                Quantity: 2,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow.AddDays(1),
                AppointmentDate: DateTime.UtcNow.AddDays(2),
                Note: "Test note"
            );
        }
    }
}