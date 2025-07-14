using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using NSubstitute;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetAllServicesByReceptionIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestServiceId = 1;

        public GetAllServicesByReceptionIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SeedData();
        }

        private void SeedData()
        {
            var now = DateTime.UtcNow;
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
                    ReceptionDate = now,
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception);
            }

            var requestForm = dbContext.RequestForms.FirstOrDefault(rf => rf.ReceptionId == TestReceptionId);
            if (requestForm == null)
            {
                requestForm = new RequestForm
                {
                    ReceptionId = TestReceptionId,
                    RequestNumber = "REQ001",
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1
                };
                dbContext.RequestForms.Add(requestForm);
                dbContext.SaveChanges();

                var serviceRequest = new ServiceRequestDetail
                {
                    RequestFormId = requestForm.Id,
                    ServiceId = TestServiceId,
                    Quantity = 1,
                    UnitPrice = 150000,
                    PaymentStatus = PaymentStatusForItem.Paid,
                    CreatedAt = now,
                    CreatedBy = 1,
                    LastUpdatedAt = now,
                    LastUpdatedBy = 1,
                    InvoiceDate = now
                };

                dbContext.ServiceRequestDetails.Add(serviceRequest);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllServices_WithValidReceptionId_ReturnsOkAndServices()
        {
            // Arrange
            var hospitalServiceMock = _factory.Services.GetRequiredService<IHospitalService>();
            hospitalServiceMock
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                new()
                {
                    Id = TestServiceId,
                    ServiceCode = "SER001",
                    ServiceName = "Khám tổng quát",
                    UnitPrice = 150000
                }
                });

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/services");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllServicesByReceptionIdResponse>();
            result.Should().NotBeNull();
            result!.Services.Should().NotBeEmpty();
            result.Services.First().ServiceId.Should().Be(TestServiceId);
            result.Services.First().ServiceName.Should().Be("Khám tổng quát");
        }

        [Fact]
        public async Task GetAllServices_WithInvalidReceptionId_ReturnsNotFound()
        {
            var invalidReceptionId = 999;

            var response = await _client.GetAsync($"/receptions/{invalidReceptionId}/services");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllServices_WithoutAuthorization_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/services");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}