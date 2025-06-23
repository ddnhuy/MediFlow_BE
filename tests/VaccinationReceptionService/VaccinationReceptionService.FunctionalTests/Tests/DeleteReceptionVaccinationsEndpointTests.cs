using System.Text;
using System.Text.Json;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class DeleteReceptionVaccinationsEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;

        public DeleteReceptionVaccinationsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                    ReceptionDate = DateTime.Now,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Create ReceptionVaccination if not exists
            var receptionVaccination = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.ReceptionId == TestReceptionId && rv.VaccineId == TestVaccineId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.Now,
                    InvoiceDate = DateTime.Now,
                    AppointmentDate = DateTime.Now,
                    IsPaid = false,
                    IsConfirmed = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task DeleteReceptionVaccinations_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var serviceIds = new List<int> { TestVaccineId };

            var request = new HttpRequestMessage(HttpMethod.Delete, "/reception-vaccinations")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(serviceIds),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteReceptionVaccinations_WithValidData_ReturnsOk()
        {
            // Arrange
            var serviceIds = new List<int> { TestVaccineId };

            var request = new HttpRequestMessage(HttpMethod.Delete, "/reception-vaccinations")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(serviceIds),
                    Encoding.UTF8,
                    "application/json")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Debug log
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {responseContent}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<DeleteReceptionVaccinationsResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();

            // Verify deletion
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deletedVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.ReceptionId == TestReceptionId && rv.VaccineId == TestVaccineId);
            deletedVaccination.Should().BeNull();
        }


        [Fact]
        public async Task DeleteReceptionVaccinations_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var serviceIds = new List<int>();

            var request = new HttpRequestMessage(HttpMethod.Delete, "/reception-vaccinations")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(serviceIds),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}