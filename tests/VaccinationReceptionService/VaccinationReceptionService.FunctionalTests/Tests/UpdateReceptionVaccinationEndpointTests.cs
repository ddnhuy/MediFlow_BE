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
                    //IsConfirmed = false,
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
            var request = CreateValidRequest();

            // Act
            var response = await _client.PutAsJsonAsync($"/receptions/{TestReceptionId}/reception-vaccinations", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = CreateValidRequest();

            // Act
            var response = await _client.PutAsJsonAsync($"/receptions/{TestReceptionId}/reception-vaccinations", request);

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
            receptionVaccination!.Quantity.Should().Be(request.Quantity);
            receptionVaccination.IsReadyToUse.Should().Be(request.IsReadyToUse);
            receptionVaccination.ScheduledDate.Should().BeCloseTo(request.ScheduledDate, TimeSpan.FromSeconds(1));
            receptionVaccination.AppointmentDate.Should().BeCloseTo(request.AppointmentDate!.Value, TimeSpan.FromSeconds(1));
            receptionVaccination.Note.Should().Be(request.Note);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithInvalidReceptionId_ReturnsNotFound()
        {
            // Arrange
            var request = CreateValidRequest();
            var invalidReceptionId = TestReceptionId + 999;

            // Act
            var response = await _client.PutAsJsonAsync($"/receptions/{invalidReceptionId}/reception-vaccinations", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateReceptionVaccination_WithRequestNumberAndRelatedServiceRequestDetail_UpdatesBothEntities()
        {
            // Arrange
            var requestNumber = "REQ-002";
            var testReceptionVaccinationId = 2;

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var receptionVaccination = new ReceptionVaccination
            {
                Id = testReceptionVaccinationId,
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
                RequestNumber = requestNumber, // Set RequestNumber
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.ReceptionVaccinations.Add(receptionVaccination);

            var serviceRequestDetail = new ServiceRequestDetail
            {
                Id = 1,
                RequestNumber = requestNumber,
                ReceptionId = TestReceptionId,
                Quantity = 1,
                PaymentStatus = PaymentStatusForItem.NotPaid,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = 1
            };
            dbContext.ServiceRequestDetails.Add(serviceRequestDetail);

            await dbContext.SaveChangesAsync();

            var request = new UpdateReceptionVaccinationRequest(
                Id: testReceptionVaccinationId,
                Quantity: 5,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow.AddDays(1),
                AppointmentDate: DateTime.UtcNow.AddDays(2),
                Note: "Test note with request number"
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/receptions/{TestReceptionId}/reception-vaccinations", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify ReceptionVaccination was updated
            var updatedReceptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == testReceptionVaccinationId);

            updatedReceptionVaccination.Should().NotBeNull();
            updatedReceptionVaccination!.Quantity.Should().Be(1);

            // Verify ServiceRequestDetail was also updated
            var updatedServiceRequestDetail = await dbContext.ServiceRequestDetails
                .FirstOrDefaultAsync(s => s.RequestNumber == requestNumber);

            updatedServiceRequestDetail.Should().NotBeNull();
            updatedServiceRequestDetail!.Quantity.Should().Be(1);
        }

        private UpdateReceptionVaccinationRequest CreateValidRequest()
        {
            return new UpdateReceptionVaccinationRequest(
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