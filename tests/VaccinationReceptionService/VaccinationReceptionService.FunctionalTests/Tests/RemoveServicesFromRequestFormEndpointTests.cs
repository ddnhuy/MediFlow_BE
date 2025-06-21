using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class RemoveServicesFromRequestFormEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestServiceId = 1;

        public RemoveServicesFromRequestFormEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Create RequestForm if not exists
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

                // Create RequestFormService if not exists
                var requestFormService = await dbContext.ServiceRequestDetails
                    .FirstOrDefaultAsync(rfs => rfs.RequestFormId == requestForm.Id && rfs.ServiceId == TestServiceId);
                if (requestFormService == null)
                {
                    requestFormService = new ServiceRequestDetail
                    {
                        RequestFormId = requestForm.Id,
                        ServiceId = TestServiceId,
                        Quantity = 1,
                        UnitPrice = 100,
                        IsCancelled = false,
                        CreatedAt = DateTime.Now,
                        CreatedBy = 1,
                        LastUpdatedAt = DateTime.Now,
                        LastUpdatedBy = 1
                    };
                    await dbContext.ServiceRequestDetails.AddAsync(requestFormService);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var serviceIds = new List<int> { TestServiceId };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithValidData_ReturnsOk()
        {
            // Arrange
            var serviceIds = new List<int> { TestServiceId };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {content}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<RemoveServicesFromRequestFormResponse>();
            result.Should().NotBeNull();
            result!.RequestFormId.Should().BeGreaterThan(0);
        }


        [Fact]
        public async Task RemoveServicesFromRequestForm_WithEmptyServiceIds_ReturnsBadRequest()
        {
            // Arrange
            var serviceIds = new List<int>(); // Empty list

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithInvalidReceptionId_ReturnsNotFound()
        {
            // Arrange
            var serviceIds = new List<int> { TestServiceId };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/request-forms/9999/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemoveServicesFromRequestForm_WithInvalidServiceId_ReturnsNotFound()
        {
            // Arrange
            var serviceIds = new List<int> { 99999 };

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/request-forms/{TestReceptionId}/services")
            {
                Content = JsonContent.Create(serviceIds)
            };
            var response = await _client.SendAsync(request);

            // Debug log
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}