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
using System.Text.Json;

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

        [Fact]
        public async Task CreatePatientReception_WithExistingPatientId_UpdatesPatient()
        {
            // Arrange
            var existingPatientId = 1;
            var command = CreateValidCommand() with { patientId = existingPatientId };

            // Mock gRPC response for existing patient
            var existingPatient = new PatientDetailModel
            {
                Id = existingPatientId,
                Name = "Existing Patient",
                Code = "PAT001"
            };

            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(existingPatient),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock
                .GetPatientAsync(Arg.Is<GetPatientRequest>(r => r.Id == existingPatientId), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Mock update patient response
            var updateResponse = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(existingPatient),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock
                .UpdatePatientAsync(Arg.Any<UpdatePatientRequest>(), Arg.Any<Metadata>())
                .Returns(updateResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/patient-reception", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<PatientReceptionResponse>();
            result.Should().NotBeNull();
            result!.patientId.Should().Be(existingPatientId);
        }

        [Fact]
        public async Task CreatePatientReception_WithExistingPatientIdButPatientNotFound_CreatesNewPatient()
        {
            // Arrange
            var nonExistentPatientId = 999;
            var command = CreateValidCommand() with { patientId = nonExistentPatientId };

            // Mock gRPC response for non-existent patient
            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult<PatientDetailModel>(null!),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock
                .GetPatientAsync(Arg.Is<GetPatientRequest>(r => r.Id == nonExistentPatientId), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Mock create patient response
            var newPatient = new PatientDetailModel
            {
                Id = 1,
                Name = "New Patient",
                Code = "PAT001"
            };

            var createResponse = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(newPatient),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock
                .CreatePatientAsync(Arg.Any<CreatePatientRequest>(), Arg.Any<Metadata>())
                .Returns(createResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/patient-reception", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<PatientReceptionResponse>();
            result.Should().NotBeNull();
            result!.patientId.Should().Be(newPatient.Id);
        }

        [Fact]
        public async Task CreatePatientReception_WithPreviousReceptionButNoUnpaidVaccinations_DoesNotMoveVaccinations()
        {
            // Arrange
            var command = CreateValidCommand();

            // Create a previous reception without unpaid vaccinations
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var previousReception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.Now.AddDays(-1),
                ServiceTypeId = TestServiceTypeId
            };
            await dbContext.Receptions.AddAsync(previousReception);
            await dbContext.SaveChangesAsync();

            // Mock gRPC response for new patient
            var newPatient = new PatientDetailModel
            {
                Id = 1,
                Name = "Test Patient",
                Code = "PAT001"
            };

            var createResponse = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(newPatient),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock
                .CreatePatientAsync(Arg.Any<CreatePatientRequest>(), Arg.Any<Metadata>())
                .Returns(createResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/patient-reception", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<PatientReceptionResponse>();
            result.Should().NotBeNull();

            // Verify no vaccinations were moved
            var vaccinations = await dbContext.ReceptionVaccinations
                .Where(rv => rv.ReceptionId == result!.receptionId)
                .ToListAsync();
            vaccinations.Should().BeEmpty();
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