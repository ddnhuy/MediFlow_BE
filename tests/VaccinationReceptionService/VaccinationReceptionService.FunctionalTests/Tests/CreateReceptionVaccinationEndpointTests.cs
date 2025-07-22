using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineStockStatus;
using BuildingBlocks.Strings.Enums;
using HumanResource.Grpc;
using Testcontainers.RabbitMq;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.VaccinationReceptions.Commands;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class CreateReceptionVaccinationEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private const int TestDoctorId = 1;
        private const int TestServiceTypeId = 1;

        public CreateReceptionVaccinationEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
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
                    ServiceTypeId = TestServiceTypeId,
                    PatientId = 1, // Thêm PatientId
                    ReceptionDate = DateTime.UtcNow,
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
        public async Task CreateReceptionVaccination_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: TestReceptionId,
                VaccineId: TestVaccineId,
                Quantity: 1,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow,
                AppointmentDate: DateTime.UtcNow,
                Note: null
            );
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithValidData_ReturnsCreated()
        {
            // Arrange
            var patientId = 1;
            var grpcResponse = new PatientDetailModel
            {
                Id = patientId,
                Code = "BN001",
                Name = "Nguyen Van A",
                Gender = 1,
                Dob = Timestamp.FromDateTime(DateTime.UtcNow),
                PhoneNumber = "0123456789",
                Email = "abcd@example.com",
                IdentityCard = "123456789",
                AddressDetail = "123 Street",
                Province = "Hanoi",
                District = "Cau Giay",
                Ward = "Dich Vong",
                IsPregnant = false,
                IsForeigner = false,
                IsSuspended = false,
                IsCancelled = false
            };

            var asyncUnaryCall = new AsyncUnaryCall<PatientDetailModel>(
                Task.FromResult(grpcResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock?
                .GetPatientAsync(Arg.Any<GetPatientRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Configure the mock from factory

            // Arrange
            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = 1,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "COVID-19",
                RouteOfAdministration = RouteOfAdministration.IM.ToString(),
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfo2 = new GetMedicineInformationResponse
            {
                MedicineId = 2,
                MedicineName = "Flu Vaccine",
                VaccineTypeName = "Influenza",
                RouteOfAdministration = RouteOfAdministration.IM.ToString(),
                MedicineTypeName = "Vaccine",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1, medicineInfo2 };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

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

            var medicineInteractionInfo = new MedicineInteractionInfo()
            {
                Id = 1,
                MedicineId1 = 1,
                Medicine1Name = "Covid 19 ",
                MedicineId2 = 2,
                Medicine2Name = "Influenza",
                HarmfulEffects = "fsadf",
                Mechanism = "dasd",
                PreventiveActions = "sdadasd",
                ReferenceInfo = "dasdsad",
                Notes = "dadsa"
            };


            _factory.InventoryServiceMock
                .GetMedicineInteractionsResponseAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(new GetMedicineInteractionsResponse()
                {
                    MedicineId = 1,
                    MedicineName = "dsadsa",
                    Interactions = new List<MedicineInteractionInfo>() { medicineInteractionInfo },
                    RequestId = "2121adsa23121",
                });

            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: TestReceptionId,
                VaccineId: TestVaccineId,
                Quantity: 1,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow.AddDays(7),
                AppointmentDate: DateTime.UtcNow.AddDays(7),
                Note: null
            );

            _factory.InventoryServiceMock
                .CheckMedicineStockResponseAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(new CheckMedicineStockResponse()
                {
                    CurrentStock = 1000,
                    Difference = 1000,
                    IsEnough = true,
                    IsSuccess = true,
                    MedicineId = 1,
                    NumberOfMedicineWanted = 10
                });

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreateReceptionVaccinationResponse>();
            result.Should().NotBeNull();
            result!.ReceptionVaccinationId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: 0,
                VaccineId: 0,
                Quantity: 0,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow,
                AppointmentDate: DateTime.UtcNow,
                Note: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            result.Should().NotBeNull();
            result!.Detail.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateReceptionVaccination_WithInvalidData_ReturnsNotFound()
        {
            // Arrange
            var command = new CreateReceptionVaccinationCommand(
                ReceptionId: 9999,
                VaccineId: TestVaccineId,
                Quantity: 1,
                IsReadyToUse: true,
                ScheduledDate: DateTime.UtcNow.AddDays(7),
                AppointmentDate: DateTime.UtcNow.AddDays(7),
                Note: null
            );

            // Act
            var response = await _client.PostAsJsonAsync("/reception-vaccinations", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}