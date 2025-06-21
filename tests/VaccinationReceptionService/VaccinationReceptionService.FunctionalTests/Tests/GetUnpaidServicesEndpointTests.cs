using System.Net;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetUnpaidServicesEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestServiceId = 1;
        private const int TestVaccineId = 1;

        public GetUnpaidServicesEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Create Service if not exists
            //var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == TestServiceId);
            //if (service == null)
            //{
            //    service = new Service
            //    {
            //        Id = TestServiceId,
            //        ServiceCode = "SVC001",
            //        ServiceName = "Test Service",
            //        UnitPrice = 100,
            //        CreatedAt = DateTime.Now,
            //        CreatedBy = 1,
            //        LastUpdatedAt = DateTime.Now,
            //        LastUpdatedBy = 1
            //    };
            //    await dbContext.Services.AddAsync(service);
            //}

            // Create RequestForm with unpaid service if not exists
            var requestForm = await dbContext.RequestForms
                .FirstOrDefaultAsync(rf => rf.ReceptionId == TestReceptionId);
            if (requestForm == null)
            {
                requestForm = new RequestForm
                {
                    ReceptionId = TestReceptionId,
                    RequestNumber = "REQ001",
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.RequestForms.AddAsync(requestForm);
                await dbContext.SaveChangesAsync();

                var requestFormService = new ServiceRequestDetail
                {
                    RequestFormId = requestForm.Id,
                    ServiceId = TestServiceId,
                    Quantity = 1,
                    UnitPrice = 100,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.ServiceRequestDetails.AddAsync(requestFormService);
            }

            // Create ReceptionVaccination with unpaid status if not exists
            var receptionVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.ReceptionId == TestReceptionId && rv.VaccineId == TestVaccineId);
            if (receptionVaccination == null)
            {
                receptionVaccination = new ReceptionVaccination
                {
                    ReceptionId = TestReceptionId,
                    VaccineId = TestVaccineId,
                    Quantity = 1,
                    PaymentStatus = PaymentStatusForItem.NotPaid,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.ReceptionVaccinations.AddAsync(receptionVaccination);
            }

            await dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetUnpaidServices_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/unpaid-services");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUnpaidServices_WithValidData_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync($"/receptions/{TestReceptionId}/unpaid-services");

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UnpaidServicesResponseDTO>();
            result.Should().NotBeNull();

            // Verify services
            result!.Services.Should().NotBeNull();
            result.Services.Should().NotBeEmpty();
            result.Services.First().ServiceId.Should().Be(TestServiceId);

            // Verify vaccinations
            result.Vaccinations.Should().NotBeNull();
            result.Vaccinations.Should().NotBeEmpty();
            result.Vaccinations.First().VaccineId.Should().Be(TestVaccineId);
        }

        [Fact]
        public async Task GetUnpaidServices_WithInvalidReceptionId_ReturnsNotFound()
        {
            var invalidReceptionId = 999;

            var response = await _client.GetAsync($"/receptions/{invalidReceptionId}/unpaid-services");

            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().NotBeNull();
            content!.Should().NotBeNullOrEmpty();
        }
    }
}