using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;
using VaccinationReceptionService.FunctionalTests.Abstractions;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class AddServiceToRequestFormEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestServiceId = 1;

        public AddServiceToRequestFormEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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

            // Create ServiceType if not exists
            var serviceType = await dbContext.ServiceTypes.FirstOrDefaultAsync(st => st.Id == 1);
            if (serviceType == null)
            {
                serviceType = new ServiceType
                {
                    Id = 1,
                    Name = "Test Service Type",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                await dbContext.ServiceTypes.AddAsync(serviceType);
            }

            // Create Reception if not exists
            var reception = await dbContext.Receptions.FirstOrDefaultAsync(r => r.Id == TestReceptionId);
            if (reception == null)
            {
                reception = new Reception
                {
                    Id = TestReceptionId,
                    ServiceTypeId = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                await dbContext.Receptions.AddAsync(reception);
            }

            // Create Service if not exists
            var service = await dbContext.Services.FirstOrDefaultAsync(s => s.Id == TestServiceId);
            if (service == null)
            {
                service = new Service
                {
                    Id = TestServiceId,
                    ServiceCode = "TEST001",
                    ServiceName = "Test Service",
                    UnitPrice = 100,
                    DepartmentId = 1,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                await dbContext.Services.AddAsync(service);
            }

            await dbContext.SaveChangesAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task AddServiceToRequestForm_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new AddServiceToRequestFormCommand(
                ReceptionId: TestReceptionId,
                Services: new List<ServiceRequestItemDTO>
                {
                    new() { ServiceId = TestServiceId, Quantity = 1 }
                },
                GroupType: null,
                GroupId: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

            var content = await response.Content.ReadAsStringAsync();
           
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<AddServiceToRequestFormResponse>();
            result.Should().NotBeNull();
            result!.RequestFormId.Should().BeGreaterThan(0);
            result.RequestNumber.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task AddServiceToRequestForm_WithInvalidReceptionId_ReturnsNotFound()
        {
            // Arrange
            var command = new AddServiceToRequestFormCommand(
                ReceptionId: 999, // Non-existent reception ID
                Services: new List<ServiceRequestItemDTO>
                {
                    new() { ServiceId = TestServiceId, Quantity = 1 }
                },
                GroupType: null,
                GroupId: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Extensions.Should().ContainKey("error");
            result.Extensions["error"]!.ToString().Should().Be("Reception không tồn tại");
        }

        [Fact]
        public async Task AddServiceToRequestForm_WithEmptyServices_ReturnsBadRequest()
        {
            // Arrange
            var command = new AddServiceToRequestFormCommand(
                ReceptionId: TestReceptionId,
                Services: new List<ServiceRequestItemDTO>(), // Empty services list
                GroupType: null,
                GroupId: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
        }
    }
}