using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Domain.Models;
using VaccinationReception.Infrastructure.Data;
using VaccinationReceptionService.FunctionalTests.Abstractions;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class PatientReceptionEndpointTests : BaseFunctionalTest, IAsyncLifetime
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestServiceTypeId = 1;

        public PatientReceptionEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
            var serviceType = await dbContext.ServiceTypes.FirstOrDefaultAsync(st => st.Id == TestServiceTypeId);
            if (serviceType == null)
            {
                serviceType = new ServiceType
                {
                    Id = TestServiceTypeId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.Now,
                    LastUpdatedBy = 1
                };
                await dbContext.ServiceTypes.AddAsync(serviceType);
                await dbContext.SaveChangesAsync();
            }
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CreatePatientReception_WhenGrpcReturnsNull_ThrowsInternalServerException()
        {
            // Arrange
            var command = CreateValidCommand();

            // Mock gRPC response to return null
            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult<PatientDetailModel>(null!),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock
                .CreatePatientAsync(Arg.Any<CreatePatientRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Act
            var response = await _client.PostAsJsonAsync("/patient-reception", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().Contain("Tạo bệnh nhân thất bại");
        }

        [Fact]
        public async Task CreatePatientReception_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = CreateInvalidCommand();

            // Act
            var response = await _client.PostAsJsonAsync("/patient-reception", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        private CreatePatientReceptionCommand CreateValidCommand()
        {
            return new CreatePatientReceptionCommand(
                new CreatePatientCommand(
                    Code: "PAT001",
                    Name: "Test Patient",
                    Gender: 1,
                    Dob: new DateTime(1990, 1, 1),
                    PhoneNumber: "0123456789",
                    IdentityCard: "123456789",
                    AddressDetail: "123 Test Street",
                    Province: "Test Province",
                    District: "Test District",
                    Ward: "Test Ward",
                    IsPregnant: false,
                    IsForeigner: false,
                    IsSuspended: false,
                    IsCancelled: false
                ),
                new CreateReceptionDTO
                {
                    PatientId = 0,
                    ReceptionDate = DateTime.Now,
                    ServiceTypeId = TestServiceTypeId
                },
                patientId: 0
            );
        }

        private CreatePatientReceptionCommand CreateInvalidCommand()
        {
            return new CreatePatientReceptionCommand(
                new CreatePatientCommand(
                    Code: "",
                    Name: "",
                    Gender: 1,
                    Dob: DateTime.Now.AddDays(1),
                    PhoneNumber: "invalid",
                    IdentityCard: "",
                    AddressDetail: "",
                    Province: "",
                    District: "",
                    Ward: "",
                    IsPregnant: false,
                    IsForeigner: false,
                    IsSuspended: false,
                    IsCancelled: false
                ),
                new CreateReceptionDTO
                {
                    PatientId = 0,
                    ReceptionDate = DateTime.Now,
                    ServiceTypeId = 0
                },
                patientId: 0
            );
        }
    }
}