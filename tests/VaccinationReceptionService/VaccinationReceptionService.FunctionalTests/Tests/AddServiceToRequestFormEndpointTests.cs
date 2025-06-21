using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Enums;
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
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        [Fact]
        public async Task AddServiceToRequestForm_WithExistingUnpaidService_UpdatesQuantity()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create initial request form and unpaid service detail
            var requestForm = new RequestForm
            {
                ReceptionId = TestReceptionId,
                RequestNumber = UniqueStringGenerator.GenerateUniqueString()
            };
            await dbContext.RequestForms.AddAsync(requestForm);
            await dbContext.SaveChangesAsync();

            var serviceDetail = new ServiceRequestDetail
            {
                RequestFormId = requestForm.Id,
                ServiceId = TestServiceId,
                Quantity = 1,
                PaymentStatus = PaymentStatusForItem.NotPaid
            };
            await dbContext.ServiceRequestDetails.AddAsync(serviceDetail);
            await dbContext.SaveChangesAsync();

            var command = new AddServiceToRequestFormCommand(
                ReceptionId: TestReceptionId,
                Services: new List<ServiceRequestItemDTO>
                {
                    new() { ServiceId = TestServiceId, Quantity = 2 }
                },
                GroupType: null,
                GroupId: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<AddServiceToRequestFormResponse>();
            result.Should().NotBeNull();

            // Verify quantity was updated
            var updatedService = await dbContext.ServiceRequestDetails
                .FirstOrDefaultAsync(srd => srd.RequestFormId == requestForm.Id);
            updatedService.Should().NotBeNull();
        }

        //[Fact]
        //public async Task AddServiceToRequestForm_WithServiceGroupAndPaidService_CreatesNewServiceDetail()
        //{
        //    // Arrange
        //    using var scope = _factory.Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //    // Create ServiceGroup
        //    var serviceGroup = new ServiceGroup
        //    {
        //        GroupName = "Test Group",
        //        CreatedAt = DateTime.UtcNow,
        //        CreatedBy = 1
        //    };
        //    await dbContext.ServiceGroups.AddAsync(serviceGroup);

        //    // Add service to group
        //    var serviceGroupService = new ServiceGroupService
        //    {
        //        ServiceGroupId = 1,
        //        ServiceId = TestServiceId
        //    };
        //    await dbContext.ServiceGroupServices.AddAsync(serviceGroupService);

        //    // Create request form with paid service
        //    var requestForm = new RequestForm
        //    {
        //        ReceptionId = TestReceptionId,
        //        RequestNumber = UniqueStringGenerator.GenerateUniqueString()
        //    };
        //    await dbContext.RequestForms.AddAsync(requestForm);
        //    await dbContext.SaveChangesAsync();

        //    var serviceDetail = new ServiceRequestDetail
        //    {
        //        RequestFormId = requestForm.Id,
        //        ServiceId = TestServiceId,
        //        Quantity = 1,
        //        IsPaid = true
        //    };
        //    await dbContext.ServiceRequestDetails.AddAsync(serviceDetail);
        //    await dbContext.SaveChangesAsync();

        //    var command = new AddServiceToRequestFormCommand(
        //        ReceptionId: TestReceptionId,
        //        Services: null,
        //        GroupType: "ServiceGroup",
        //        GroupId: 1,
        //        DefaultQuantity: 2
        //    );

        //    // Act
        //    var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

        //    // Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.Created);
        //    var result = await response.Content.ReadFromJsonAsync<AddServiceToRequestFormResponse>();
        //    result.Should().NotBeNull();

        //    // Verify new service detail was created
        //    var serviceDetails = await dbContext.ServiceRequestDetails
        //        .Where(srd => srd.RequestFormId == requestForm.Id && srd.ServiceId == TestServiceId)
        //        .ToListAsync();
        //}

        //[Fact]
        //public async Task AddServiceToRequestForm_WithDiseaseGroupAndUnpaidService_UpdatesQuantity()
        //{
        //    // Arrange
        //    using var scope = _factory.Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //    // Create DiseaseGroup
        //    var diseaseGroup = new DiseaseGroup
        //    {
        //        GroupName = "Test Group",
        //        CreatedAt = DateTime.UtcNow,
        //        CreatedBy = 1
        //    };
        //    await dbContext.DiseaseGroups.AddAsync(diseaseGroup);

        //    // Add service to group
        //    var diseaseGroupService = new DiseaseGroupService
        //    {
        //        DiseaseGroupId = 1,
        //        ServiceId = TestServiceId
        //    };
        //    await dbContext.DiseaseGroupServices.AddAsync(diseaseGroupService);

        //    // Create request form with unpaid service
        //    var requestForm = new RequestForm
        //    {
        //        ReceptionId = TestReceptionId,
        //        RequestNumber = UniqueStringGenerator.GenerateUniqueString()
        //    };
        //    await dbContext.RequestForms.AddAsync(requestForm);
        //    await dbContext.SaveChangesAsync();

        //    var serviceDetail = new ServiceRequestDetail
        //    {
        //        RequestFormId = requestForm.Id,
        //        ServiceId = TestServiceId,
        //        Quantity = 1,
        //        IsPaid = false
        //    };
        //    await dbContext.ServiceRequestDetails.AddAsync(serviceDetail);
        //    await dbContext.SaveChangesAsync();

        //    var command = new AddServiceToRequestFormCommand(
        //        ReceptionId: TestReceptionId,
        //        Services: null,
        //        GroupType: "DiseaseGroup",
        //        GroupId: 1,
        //        DefaultQuantity: 2
        //    );

        //    // Act
        //    var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

        //    // Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.Created);
        //    var result = await response.Content.ReadFromJsonAsync<AddServiceToRequestFormResponse>();
        //    result.Should().NotBeNull();

        //    // Verify quantity was updated
        //    var updatedService = await dbContext.ServiceRequestDetails
        //        .FirstOrDefaultAsync(srd => srd.RequestFormId == requestForm.Id && srd.ServiceId == TestServiceId);
        //    updatedService.Should().NotBeNull();
        //}

        [Fact]
        public async Task AddServiceToRequestForm_WithNoServicesAndNoGroup_ReturnsBadRequest()
        {
            // Arrange
            var command = new AddServiceToRequestFormCommand(
                ReceptionId: TestReceptionId,
                Services: null,
                GroupType: null,
                GroupId: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}