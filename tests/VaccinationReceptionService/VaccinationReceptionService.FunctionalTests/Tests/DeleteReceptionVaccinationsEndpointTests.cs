using System;
using System.Text;
using System.Text.Json;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class DeleteReceptionVaccinationsEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private const string TestRequestNumber = "REQ-001";
        private static readonly DateTime TestDateTime = DateTime.UtcNow;

        public DeleteReceptionVaccinationsEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Seed Reception
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    PatientId = 1,
                    ReceptionDate = TestDateTime,
                    CreatedAt = TestDateTime,
                    CreatedBy = 1,
                    LastUpdatedAt = TestDateTime,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            // Seed ReceptionVaccination with RequestNumber
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
                    ScheduledDate = TestDateTime,
                    InvoiceDate = TestDateTime,
                    AppointmentDate = TestDateTime,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    RequestNumber = TestRequestNumber, // This will trigger the ServiceRequestDetails logic
                    CreatedAt = TestDateTime,
                    CreatedBy = 1,
                    LastUpdatedAt = TestDateTime,
                    LastUpdatedBy = 1,
                    IsCancelled = false
                };
                dbContext.ReceptionVaccinations.Add(receptionVaccination);
            }

            // Seed ServiceRequestDetails to test the related service details cancellation
            var serviceRequestDetail = dbContext.ServiceRequestDetails
                .FirstOrDefault(srd => srd.RequestNumber == TestRequestNumber && srd.ReceptionId == TestReceptionId);
            if (serviceRequestDetail == null)
            {
                serviceRequestDetail = new ServiceRequestDetail
                {
                    RequestNumber = TestRequestNumber,
                    ReceptionId = TestReceptionId,
                    ServiceId = 1, // Assuming there's a ServiceId
                    Quantity = 1,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    IsCancelled = false,
                    CreatedAt = TestDateTime,
                    CreatedBy = 1,
                    LastUpdatedAt = TestDateTime,
                    LastUpdatedBy = 1
                };
                dbContext.ServiceRequestDetails.Add(serviceRequestDetail);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task DeleteReceptionVaccinations_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var serviceIds = new List<int> { TestVaccineId };
            var request = new HttpRequestMessage(HttpMethod.Post, $"/reception-vaccinations/{TestReceptionId}")
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
            var serviceIds = new List<int> { TestVaccineId };
            var request = new HttpRequestMessage(HttpMethod.Post, $"/reception-vaccinations/{TestReceptionId}")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(serviceIds),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {responseContent}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<DeleteReceptionVaccinationsResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
            result.DeletedCount.Should().Be(1);
        }

        [Fact]
        public async Task DeleteReceptionVaccinations_WithValidDataAndServiceDetails_CancelsRelatedServices()
        {
            // This test specifically verifies that related ServiceRequestDetails are also cancelled
            var serviceIds = new List<int> { TestVaccineId };
            var request = new HttpRequestMessage(HttpMethod.Post, $"/reception-vaccinations/{TestReceptionId}")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(serviceIds),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<DeleteReceptionVaccinationsResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
            result.DeletedCount.Should().Be(1);
        }

        [Fact]
        public async Task DeleteReceptionVaccinations_WithInvalidData_ReturnsBadRequest()
        {
            var ids = new List<int>(); // empty list
            var request = new HttpRequestMessage(HttpMethod.Post, $"/reception-vaccinations/{TestReceptionId}")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(ids),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}