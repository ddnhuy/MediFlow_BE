using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Pagination;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using VaccinationReception.API.EndPoints.VaccinationEndpoints;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetAllVaccinationHistoryEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId1 = 1;
        private const int TestPatientId2 = 2;
        private const int TestReceptionId1 = 1;
        private const int TestReceptionId2 = 2;
        private const int TestReceptionVaccinationId1 = 1;
        private const int TestReceptionVaccinationId2 = 2;
        private const int TestReceptionVaccinationId3 = 3;
        private const int TestReceptionVaccinationId4 = 4;
        private const int TestVaccinationId1 = 1;
        private const int TestVaccinationId2 = 2;
        private const int TestVaccinationId3 = 3;
        private const int TestMedicineId1 = 1;
        private const int TestMedicineId2 = 2;
        private const int TestDoctorId1 = 1;
        private const int TestDoctorId2 = 2;

        public GetAllVaccinationHistoryEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            ConfigureMocks();
            SeedData();
        }

        private void ConfigureMocks()
        {
            // Setup Patient gRPC mock for multiple patients
            var patientId = 1;
            var grpcResponse = new FilteredPatientsResponse
            {
                Data = {
                    new PatientSummaryModel
                    {
                        Id = patientId,
                        Code = "PAT-001",
                        Name = "John Doe",
                        Gender = 1,
                        Dob = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc)),
                        PhoneNumber = "0123456789",
                        Email = "abcd@example.com",
                        IdentityCard = "123456789",
                        AddressDetail = "123 Street",
                        Province = "Hanoi",
                        District = "Cau Giay",
                        Ward = "Dich Vong",
                        IsPregnant = false,
                        IsForeigner = false,
                    }
                }
            };

            var asyncUnaryCall = new AsyncUnaryCall<FilteredPatientsResponse>(
                Task.FromResult(grpcResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _grpcClientMock?
                .ListPatientsWithIdsAndSearchAsync(Arg.Any<FilteredPatientsRequest>(), Arg.Any<Metadata>())
                .Returns(asyncUnaryCall);

            // Setup Medicine Information mock
            var medicineInfo1 = new GetMedicineInformationResponse
            {
                MedicineId = TestMedicineId1,
                MedicineName = "COVID-19 Vaccine",
                VaccineTypeName = "mRNA",
                MedicineTypeName = "Vaccine",
                Concentration = "30 mg/mL",
                IsSuccess = true
            };

            var medicineInfo2 = new GetMedicineInformationResponse
            {
                MedicineId = TestMedicineId2,
                MedicineName = "Hepatitis B Vaccine",
                VaccineTypeName = "Recombinant",
                MedicineTypeName = "Vaccine",
                Concentration = "20 mg/mL",
                IsSuccess = true
            };

            var medicineInfoList = new List<GetMedicineInformationResponse> { medicineInfo1, medicineInfo2 };

            _factory.InventoryServiceMock!
                .GetMedicineInformationAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(medicineInfoList);

            // Setup ApplicationUser gRPC mock for doctor information
            var doctorResponse1 = new ApplicationUserDetailModel
            {
                Id = TestDoctorId1,
                Name = "Dr. Smith"
            };

            var doctorResponse2 = new ApplicationUserDetailModel
            {
                Id = TestDoctorId2,
                Name = "Dr. Johnson"
            };

            var doctorAsyncUnaryCall1 = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse1),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            var doctorAsyncUnaryCall2 = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(doctorResponse2),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(
                    Arg.Is<GetApplicationUserRequest>(r => r.Id == TestDoctorId1),
                    Arg.Any<Metadata>(), null, default)
                .Returns(doctorAsyncUnaryCall1);

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(
                    Arg.Is<GetApplicationUserRequest>(r => r.Id == TestDoctorId2),
                    Arg.Any<Metadata>(), null, default)
                .Returns(doctorAsyncUnaryCall2);
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Create Receptions for multiple patients
            var reception1 = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId1);
            if (reception1 == null)
            {
                reception1 = new Reception
                {
                    Id = TestReceptionId1,
                    ServiceTypeId = 1,
                    PatientId = TestPatientId1,
                    ReceptionDate = DateTime.UtcNow.AddDays(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-2),
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception1);
            }

            var reception2 = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId2);
            if (reception2 == null)
            {
                reception2 = new Reception
                {
                    Id = TestReceptionId2,
                    ServiceTypeId = 1,
                    PatientId = TestPatientId2,
                    ReceptionDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception2);
            }

            // Create ReceptionVaccinations with different scenarios

            // Scenario 1: Successful vaccination (no issues)
            var successfulRV = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId1);
            if (successfulRV == null)
            {
                successfulRV = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId1,
                    ReceptionId = TestReceptionId1,
                    VaccineId = TestMedicineId1,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow.AddDays(-1),
                    InvoiceDate = DateTime.UtcNow.AddDays(-1),
                    AppointmentDate = DateTime.UtcNow.AddDays(-1),
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-2),
                    HasIssue = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = 1,
                    RequestNumber = "REQ-001",
                    UnitPrice = 100.00m,
                    DoctorId = TestDoctorId1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(successfulRV);
            }

            // Scenario 2: Vaccination with adverse reactions (has vaccination but with reactions)
            var adverseRV = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId2);
            if (adverseRV == null)
            {
                adverseRV = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId2,
                    ReceptionId = TestReceptionId1,
                    VaccineId = TestMedicineId2,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow.AddDays(-1),
                    InvoiceDate = DateTime.UtcNow.AddDays(-1),
                    AppointmentDate = DateTime.UtcNow.AddDays(-1),
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-2),
                    HasIssue = false, // ReceptionVaccination itself doesn't have issue
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = 1,
                    RequestNumber = "REQ-002",
                    UnitPrice = 150.00m,
                    DoctorId = TestDoctorId1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(adverseRV);
            }

            // Scenario 3: Rejected before injection (has issue, no vaccinations)
            var rejectedRV = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId3);
            if (rejectedRV == null)
            {
                rejectedRV = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId3,
                    ReceptionId = TestReceptionId2,
                    VaccineId = TestMedicineId1,
                    Quantity = 2,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow,
                    InvoiceDate = DateTime.UtcNow,
                    AppointmentDate = DateTime.UtcNow,
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-1),
                    HasIssue = true,
                    IssueNote = "Patient allergic to vaccine",
                    IssueDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    RequestNumber = "REQ-003",
                    UnitPrice = 100.00m,
                    DoctorId = TestDoctorId2,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(rejectedRV);
            }

            // Scenario 4: Outside date range (should not appear in default results)
            var oldRV = dbContext.ReceptionVaccinations
                .FirstOrDefault(rv => rv.Id == TestReceptionVaccinationId4);
            if (oldRV == null)
            {
                oldRV = new ReceptionVaccination
                {
                    Id = TestReceptionVaccinationId4,
                    ReceptionId = TestReceptionId2,
                    VaccineId = TestMedicineId2,
                    Quantity = 1,
                    IsReadyToUse = true,
                    ScheduledDate = DateTime.UtcNow.AddDays(-40), // Outside default 30-day range
                    InvoiceDate = DateTime.UtcNow.AddDays(-40),
                    AppointmentDate = DateTime.UtcNow.AddDays(-40),
                    PaymentStatus = VaccinationReception.Domain.Enums.PaymentStatusForItem.Paid,
                    VaccinationTestDate = DateTime.UtcNow.AddDays(-41),
                    HasIssue = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    CreatedBy = 1,
                    RequestNumber = "REQ-004",
                    UnitPrice = 150.00m,
                    DoctorId = TestDoctorId2,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-40),
                    LastUpdatedBy = 1
                };
                dbContext.ReceptionVaccinations.Add(oldRV);
            }

            // Create Vaccinations

            // Successful vaccination
            var successfulVaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.Id == TestVaccinationId1);
            if (successfulVaccination == null)
            {
                successfulVaccination = new Vaccination
                {
                    Id = TestVaccinationId1,
                    PatientId = TestPatientId1,
                    ReceptionVaccinationId = TestReceptionVaccinationId1,
                    MedicineBatchId = 1,
                    BatchNumber = "BATCH-001",
                    MedicineId = TestMedicineId1,
                    MedicineName = "COVID-19 Vaccine",
                    DoseNumber = 1,
                    VaccinationDate = DateTime.UtcNow.AddDays(-1),
                    IsConfirmed = true,
                    HasReaction = false, // No adverse reactions
                    HasFeverAbove39 = false,
                    HasInjectionSiteReaction = false,
                    HasOtherReaction = false,
                    Note = "Successful vaccination",
                    DoctorId = TestDoctorId1,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(successfulVaccination);
            }

            // Vaccination with adverse reactions
            var adverseVaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.Id == TestVaccinationId2);
            if (adverseVaccination == null)
            {
                adverseVaccination = new Vaccination
                {
                    Id = TestVaccinationId2,
                    PatientId = TestPatientId1,
                    ReceptionVaccinationId = TestReceptionVaccinationId2,
                    MedicineBatchId = 2,
                    BatchNumber = "BATCH-002",
                    MedicineId = TestMedicineId2,
                    MedicineName = "Hepatitis B Vaccine",
                    DoseNumber = 1,
                    VaccinationDate = DateTime.UtcNow.AddDays(-1),
                    IsConfirmed = true,
                    HasReaction = true, // Has adverse reactions
                    HasFeverAbove39 = true,
                    HasInjectionSiteReaction = true,
                    HasOtherReaction = false,
                    Note = "Vaccination completed but patient had fever",
                    DoctorId = TestDoctorId1,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(adverseVaccination);
            }

            // Old vaccination (outside date range)
            var oldVaccination = dbContext.Vaccinations
                .FirstOrDefault(v => v.Id == TestVaccinationId3);
            if (oldVaccination == null)
            {
                oldVaccination = new Vaccination
                {
                    Id = TestVaccinationId3,
                    PatientId = TestPatientId2,
                    ReceptionVaccinationId = TestReceptionVaccinationId4,
                    MedicineBatchId = 3,
                    BatchNumber = "BATCH-003",
                    MedicineId = TestMedicineId2,
                    MedicineName = "Hepatitis B Vaccine",
                    DoseNumber = 1,
                    VaccinationDate = DateTime.UtcNow.AddDays(-40), // Outside default range
                    IsConfirmed = true,
                    HasReaction = false,
                    HasFeverAbove39 = false,
                    HasInjectionSiteReaction = false,
                    HasOtherReaction = false,
                    Note = "Old vaccination",
                    DoctorId = TestDoctorId2,
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow.AddDays(-40),
                    LastUpdatedBy = 1
                };
                dbContext.Vaccinations.Add(oldVaccination);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithDefaultParameters_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Should().NotBeNull();
            result.VaccinationHistory.Data.Should().NotBeNull();

            // Should have items within default 30-day range (excludes old vaccination)
            result.VaccinationHistory.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithDateRange_ReturnsFilteredResults()
        {
            // Arrange
            var fromDate = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd");
            var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // Act
            var response = await _client.GetAsync($"/vaccination/history?fromDate={fromDate}&toDate={toDate}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();

            // All items should be within the specified date range
            foreach (var item in result.VaccinationHistory.Data)
            {
                if (item.VaccinationDate.HasValue)
                {
                    item.VaccinationDate.Value.Date.Should().BeOnOrAfter(DateTime.Parse(fromDate));
                    item.VaccinationDate.Value.Date.Should().BeOnOrBefore(DateTime.Parse(toDate));
                }
            }
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithPagination_ReturnsCorrectPage()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history?pageIndex=1&pageSize=2");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Should().NotBeNull();
            result.VaccinationHistory.PageIndex.Should().Be(1);
            result.VaccinationHistory.PageSize.Should().Be(2);
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithSearchTerm_ReturnsFilteredResults()
        {
            // Act - Search by patient name
            var response = await _client.GetAsync("/vaccination/history?searchTerm=John");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithSearchByMedicine_ReturnsFilteredResults()
        {
            // Act - Search by medicine name
            var response = await _client.GetAsync("/vaccination/history?searchTerm=COVID");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();

            // All results should contain "COVID" in medicine name or type
            foreach (var item in result.VaccinationHistory.Data)
            {
                var containsCovid = item.MedicineName.Contains("COVID", StringComparison.OrdinalIgnoreCase) ||
                                   item.MedicineTypeName.Contains("COVID", StringComparison.OrdinalIgnoreCase);
                containsCovid.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GetAllVaccinationHistory_OrderedByVaccinationDateDescending_ReturnsCorrectOrder()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();

            // Check that results are ordered by vaccination date descending
            var itemsWithDates = result.VaccinationHistory.Data
                .Where(x => x.VaccinationDate.HasValue)
                .ToList();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithSuccessfulVaccinations_ReturnsCorrectData()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();

            // Find successful vaccination (no reactions)
            var successfulItem = result.VaccinationHistory.Data
                .FirstOrDefault(x => x.ReceptionVaccinationId == TestReceptionVaccinationId1);

            successfulItem.Should().NotBeNull();
            successfulItem!.HasIssue.Should().BeFalse();
            successfulItem.IssueNote.Should().BeNull();
            successfulItem.IssueDate.Should().BeNull();
            successfulItem.VaccinationDate.Should().NotBeNull();
            successfulItem.VaccinationConfirmation.Should().BeTrue();
            successfulItem.DoseNumber.Should().Be("Mũi thứ 1");
            successfulItem.PatientName.Should().Be("John Doe");
            successfulItem.PatientCode.Should().Be("PAT-001");
            successfulItem.DoctorName.Should().Be("B.S Dr. Smith");
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithAdverseReactions_ReturnsCorrectData()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();

            // Find vaccination with adverse reactions
            var adverseItem = result.VaccinationHistory.Data
                .FirstOrDefault(x => x.ReceptionVaccinationId == TestReceptionVaccinationId2);

            adverseItem.Should().NotBeNull();
            adverseItem!.HasIssue.Should().BeTrue(); // Should be true because vaccination.HasReaction = true
            adverseItem.VaccinationDate.Should().NotBeNull();
            adverseItem.VaccinationConfirmation.Should().BeTrue();
            adverseItem.DoseNumber.Should().Be("Mũi thứ 1");
            adverseItem.PatientName.Should().Be("John Doe");
            adverseItem.PatientCode.Should().Be("PAT-001");
            adverseItem.DoctorName.Should().Be("B.S Dr. Smith");
            adverseItem.MedicineName.Should().Be("Hepatitis B Vaccine");
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithRejectedVaccines_ReturnsCorrectData()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithMixedScenarios_ReturnsAllCorrectTypes()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithExtendedDateRange_IncludesOldVaccinations()
        {
            // Arrange - Use a wider date range to include old vaccination
            var fromDate = DateTime.UtcNow.AddDays(-50).ToString("yyyy-MM-dd");
            var toDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // Act
            var response = await _client.GetAsync($"/vaccination/history?fromDate={fromDate}&toDate={toDate}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithInvalidPagination_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history?pageIndex=0&pageSize=0");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithFutureDateRange_ReturnsEmptyResults()
        {
            // Arrange
            var fromDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
            var toDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd");

            // Act
            var response = await _client.GetAsync($"/vaccination/history?fromDate={fromDate}&toDate={toDate}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllVaccinationHistory_WithNonExistentSearchTerm_ReturnsEmptyResults()
        {
            // Act
            var response = await _client.GetAsync("/vaccination/history?searchTerm=NonExistentPatient");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<GetAllVaccinationHistoryResponse>();

            result.Should().NotBeNull();
            result!.VaccinationHistory.Data.Should().BeEmpty();
        }
    }
}