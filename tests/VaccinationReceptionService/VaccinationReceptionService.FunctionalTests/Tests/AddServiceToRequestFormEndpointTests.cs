using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using NSubstitute;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class AddServiceToRequestFormEndpointTests : BaseFunctionalTest
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

            SeedData();
        }

        private void SeedData()
        {
            // Seed test data before running tests
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create ServiceType if not exists
            var serviceType = dbContext.ServiceTypes.FirstOrDefault(st => st.Id == 1);
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
                dbContext.ServiceTypes.Add(serviceType);
            }

            // Create Reception if not exists
            var reception = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId);
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
                dbContext.Receptions.Add(reception);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task AddServiceToRequestForm_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new AddServiceToRequestFormCommand(
                ReceptionId: TestReceptionId,
                Services: new List<ServiceRequestItemDTO>
                {
                    new ServiceRequestItemDTO
                    {
                        ServiceId = TestServiceId,
                        Quantity = 1
                    }
                },
                GroupType: null,
                GroupId: null
            );

            // Mock HospitalService
            var hospitalServiceMock = _factory.Services.GetRequiredService<IHospitalService>();
            hospitalServiceMock
                .GetServicesByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = TestServiceId,
                        UnitPrice = 100000
                    }
                });

            // Act
            var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

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

        //[Fact]
        //public async Task AddServiceToRequestForm_WithExistingUnpaidService_UpdatesQuantity()
        //{
        //    // Arrange
        //    using var scope = _factory.Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //    // Create initial request form and unpaid service detail
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
        //        PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid
        //    };
        //    await dbContext.ServiceRequestDetails.AddAsync(serviceDetail);
        //    await dbContext.SaveChangesAsync();

        //    var command = new AddServiceToRequestFormCommand(
        //        ReceptionId: TestReceptionId,
        //        Services: new List<ServiceRequestItemDTO>
        //        {
        //            new() { ServiceId = TestServiceId, Quantity = 2 }
        //        },
        //        GroupType: null,
        //        GroupId: null
        //    );

        //    // Act
        //    var response = await _client.PostAsJsonAsync("/request-forms/add-service", command);

        //    // Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.Created);
        //    var result = await response.Content.ReadFromJsonAsync<AddServiceToRequestFormResponse>();
        //    result.Should().NotBeNull();

        //    // Verify quantity was updated
        //    var updatedService = await dbContext.ServiceRequestDetails
        //        .FirstOrDefaultAsync(srd => srd.RequestFormId == requestForm.Id);
        //    updatedService.Should().NotBeNull();
        //}

        //[Fact]
        //public async Task AddServiceToRequestForm_WithServiceGroupAndPaidService_CreatesNewServiceDetail()
        //{
        //    // Arrange
        //    using var scope = _factory.Services.CreateScope();
        //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();         

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
        //        PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid
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
        //        PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.NotPaid
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