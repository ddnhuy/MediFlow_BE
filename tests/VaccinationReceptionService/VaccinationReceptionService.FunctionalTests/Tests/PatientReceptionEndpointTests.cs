using BuildingBlocks.Strings;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class PatientReceptionEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestServiceTypeId = 1;

        public PatientReceptionEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
            var serviceType = dbContext.ServiceTypes.FirstOrDefault(st => st.Id == TestServiceTypeId);
            if (serviceType == null)
            {
                serviceType = new ServiceType
                {
                    Id = TestServiceTypeId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ServiceTypes.Add(serviceType);
                dbContext.SaveChanges();
            }
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
            result!.Detail.Should().Contain(ExceptionKey.FAILED_CREATE_PATIENT.ToString());
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
            _factory.HospitalServiceMock
                .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceCode = "ExamFee",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    },
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 2,
                        ServiceCode = "IM",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    }
                });
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
            _factory.HospitalServiceMock
                .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceCode = "ExamFee",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    },
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 2,
                        ServiceCode = "IM",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    }
                });
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
            _factory.HospitalServiceMock
                .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceCode = "ExamFee",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    },
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 2,
                        ServiceCode = "IM",
                        ServiceName = "dasdsa",
                        UnitPrice = 100000
                    }
                });
            // Arrange
            var command = CreateValidCommand();

            // Create a previous reception without unpaid vaccinations
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var previousReception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow.AddDays(-1),
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

        [Fact]
        public async Task CreatePatientReception_WithExamFeeService_CreatesServiceRequestDetail()
        {
            // Arrange
            var command = CreateValidCommand();

            // Mock hospital service to return exam fee service
            _factory.HospitalServiceMock
                .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 100,
                        ServiceCode = "EXAMFEE",
                        ServiceName = "Exam Fee Service",
                        UnitPrice = 50000
                    }
                });

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

            // Verify ServiceRequestDetail was created
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var serviceDetail = await dbContext.ServiceRequestDetails
                .FirstOrDefaultAsync(d => d.ReceptionId == result!.receptionId && d.ServiceId == 100);

            serviceDetail.Should().NotBeNull();
            serviceDetail!.Quantity.Should().Be(1);
            serviceDetail.UnitPrice.Should().Be(50000);
            serviceDetail.RequestNumber.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreatePatientReception_WithExistingServiceRequestDetail_DoesNotCreateDuplicate()
        {
            // Arrange
            var command = CreateValidCommand();

            // Mock hospital service to return exam fee service
            _factory.HospitalServiceMock
                .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 100,
                        ServiceCode = "EXAMFEE",
                        ServiceName = "Exam Fee Service",
                        UnitPrice = 50000
                    }
                });

            // Pre-create a reception and service detail
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var preReception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow,
                ServiceTypeId = TestServiceTypeId
            };
            await dbContext.Receptions.AddAsync(preReception);
            await dbContext.SaveChangesAsync();

            var existingDetail = new ServiceRequestDetail
            {
                RequestNumber = "REQ001",
                ReceptionId = preReception.Id,
                ServiceId = 100,
                Quantity = 1,
                UnitPrice = 50000
            };
            await dbContext.ServiceRequestDetails.AddAsync(existingDetail);
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

            // Verify no duplicate ServiceRequestDetail was created for the new reception
            var serviceDetailsCount = await dbContext.ServiceRequestDetails
                .CountAsync(d => d.ServiceId == 100);
            serviceDetailsCount.Should().Be(3); // Only the pre-existing one
        }

        [Fact]
        public async Task CreatePatientReception_WithPreviousReceptionAndPaidVaccinations_MovesVaccinations()
        {
            // Arrange
            var command = CreateValidCommand();

            // Mock hospital service
            _factory.HospitalServiceMock
                .GetServicesByServiceCodeAsync(Arg.Any<List<string>>(), Arg.Any<CancellationToken>())
                .Returns(new List<BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO>
                {
                    new BuildingBlocks.Messaging.Contracts.HospitalService.ServiceDTO
                    {
                        Id = 1,
                        ServiceCode = "EXAMFEE",
                        ServiceName = "Exam Fee Service",
                        UnitPrice = 100000
                    }
                });

            // Setup test data
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create previous reception
            var previousReception = new Reception
            {
                PatientId = 1,
                ReceptionDate = DateTime.UtcNow.AddDays(-5),
                ServiceTypeId = TestServiceTypeId
            };
            await dbContext.Receptions.AddAsync(previousReception);
            await dbContext.SaveChangesAsync();

            // Create paid vaccination with future appointment date
            var futureAppointmentDate = DateTime.UtcNow.AddDays(5);
            var paidVaccination = new ReceptionVaccination
            {
                RequestNumber = UniqueStringGenerator.GenerateUniqueString(),
                ReceptionId = previousReception.Id,
                VaccineId = 1,
                Quantity = 2,
                PaymentStatus = PaymentStatusForItem.Paid,
                AppointmentDate = futureAppointmentDate,
                UnitPrice = 200000
            };
            await dbContext.ReceptionVaccinations.AddAsync(paidVaccination);
            await dbContext.SaveChangesAsync();

            var vaccination1 = new Vaccination
            {
                ReceptionVaccinationId = paidVaccination.Id,
                MedicineId = 1,
                IsConfirmed = false,
                VaccinationDate = futureAppointmentDate
            };
            await dbContext.Vaccinations.AddAsync(vaccination1);
            await dbContext.SaveChangesAsync();

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

            // Verify vaccination was moved to new reception
            var updatedVaccination = await dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == paidVaccination.Id);
            updatedVaccination!.SecondaryReceptionId.Should().BeNull();
        }

        private CreatePatientReceptionCommand CreateValidCommand()
        {
            return new CreatePatientReceptionCommand(
                new CreatePatientCommand(
                    Code: "PAT001",
                    Name: "Test Patient",
                    Gender: 1,
                    Dob:  new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber: "0123456789",
                    Email: "abcde@example.com",
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
                    ReceptionDate = DateTime.UtcNow.AddDays(2),
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
                    Dob: DateTime.UtcNow.AddDays(1),
                    PhoneNumber: "invalid",
                    Email: "abcde@example.com",
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
                    ReceptionDate = DateTime.UtcNow,
                    ServiceTypeId = 0
                },
                patientId: 0
            );
        }
    }
}