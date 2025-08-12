using BuildingBlocks.Pagination;
using System.Net;
using System.Text.Json;
using VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetRecentReceptionsWithPatientsEndpointTests : GetListPostVaccinationPatientBaseTest
    {
        private readonly string _testToken;
        private readonly GetListPostVaccinationPatientTestFactory _factory;
        private const int TestReceptionId1 = 1;
        private const int TestReceptionId2 = 2;
        private const int TestReceptionId3 = 3;
        private const int TestPatientId1 = 1;
        private const int TestPatientId2 = 2;
        private const int TestPatientId3 = 3;
        private const int TestServiceTypeId = 1;

        public GetRecentReceptionsWithPatientsEndpointTests(GetListPostVaccinationPatientTestFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            ConfigureMocks();
            SeedData();
        }

        private void ConfigureMocks()
        {
            // Create mock patient data
            var patientsResponse = new List<PatientSummaryDTO>
            {
                new PatientSummaryDTO
                {
                    Id = TestPatientId1,
                    Code = "BN001",
                    Name = "Nguyen Van A",
                    IdentityCard = "123456789",
                    PhoneNumber = "0123456789",
                    DOB = new DateTime(1990, 1, 1),
                    Gender = 1,
                    Province = "Hanoi",
                    District = "Cau Giay",
                    Ward = "Mai Dich",
                    AddressDetail = "123 Test Street",
                    IsPregnant = false,
                    IsForeigner = false
                },
                new PatientSummaryDTO
                {
                    Id = TestPatientId2,
                    Code = "BN002",
                    Name = "Tran Thi B",
                    IdentityCard = "987654321",
                    PhoneNumber = "0987654321",
                    DOB = new DateTime(1985, 5, 15),
                    Gender = 0,
                    Province = "Hanoi",
                    District = "Ba Dinh",
                    Ward = "Phuc Xa",
                    AddressDetail = "456 Test Avenue",
                    IsPregnant = false,
                    IsForeigner = false
                },
                new PatientSummaryDTO
                {
                    Id = TestPatientId3,
                    Code = "BN003",
                    Name = "Le Van C",
                    IdentityCard = "555666777",
                    PhoneNumber = "0555666777",
                    DOB = new DateTime(1995, 10, 20),
                    Gender = 1,
                    Province = "Ho Chi Minh",
                    District = "District 1",
                    Ward = "Ben Nghe",
                    AddressDetail = "789 Test Road",
                    IsPregnant = false,
                    IsForeigner = false
                }
            };

            // Mock the gRPC client call
            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(patientsResponse);
        }

        private void SeedData()
        {
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
            }

            // Create test receptions - some recent, some old
            var recentTime = DateTime.UtcNow.AddMinutes(-30); // 30 minutes ago (within 2 hours)
            var oldTime = DateTime.UtcNow.AddHours(-3); // 3 hours ago (outside 2 hours)

            // Recent reception 1
            var reception1 = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId1);
            if (reception1 == null)
            {
                reception1 = new Reception
                {
                    Id = TestReceptionId1,
                    ServiceTypeId = TestServiceTypeId,
                    PatientId = TestPatientId1,
                    ReceptionDate = recentTime,
                    IsCancelled = false,
                    IsSuspended = false,
                    CreatedAt = recentTime,
                    CreatedBy = 1,
                    LastUpdatedAt = recentTime,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception1);
            }

            // Recent reception 2
            var reception2 = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId2);
            if (reception2 == null)
            {
                reception2 = new Reception
                {
                    Id = TestReceptionId2,
                    ServiceTypeId = TestServiceTypeId,
                    PatientId = TestPatientId2,
                    ReceptionDate = recentTime.AddMinutes(-10),
                    IsCancelled = false,
                    IsSuspended = false,
                    CreatedAt = recentTime.AddMinutes(-10),
                    CreatedBy = 1,
                    LastUpdatedAt = recentTime.AddMinutes(-10),
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception2);
            }

            // Old reception (should not appear in results)
            var reception3 = dbContext.Receptions.FirstOrDefault(r => r.Id == TestReceptionId3);
            if (reception3 == null)
            {
                reception3 = new Reception
                {
                    Id = TestReceptionId3,
                    ServiceTypeId = TestServiceTypeId,
                    PatientId = TestPatientId3,
                    ReceptionDate = oldTime,
                    IsCancelled = false,
                    IsSuspended = false,
                    CreatedAt = oldTime,
                    CreatedBy = 1,
                    LastUpdatedAt = oldTime,
                    LastUpdatedBy = 1
                };
                dbContext.Receptions.Add(reception3);
            }

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithValidRequest_ReturnsOkWithPaginatedResult()
        {
            // Act
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Should().NotBeNull();
            result.Receptions.Data.Should().NotBeEmpty();
            result.Receptions.PageIndex.Should().Be(1);
            result.Receptions.PageSize.Should().Be(10);

            // Verify the data contains patient information
            var firstReception = result.Receptions.Data.First();
            firstReception.Patient.Should().NotBeNull();
            firstReception.Patient.Code.Should().NotBeNullOrEmpty();
            firstReception.Patient.Name.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithSearchTerm_ReturnsFilteredResults()
        {
            // Act - Search for patient with name containing "Nguyen"
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10&searchTerm=Nguyen");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().HaveCount(1); // Only one patient matches "Nguyen"
            result.Receptions.Data.First().Patient.Name.Should().Contain("Nguyen");
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithSearchByPhoneNumber_ReturnsCorrectResult()
        {
            // Act - Search by phone number
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10&searchTerm=0987654321");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().HaveCount(1);
            result.Receptions.Data.First().Patient.PhoneNumber.Should().Be("0987654321");
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithSearchByIdentityCard_ReturnsCorrectResult()
        {
            // Act - Search by identity card
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10&searchTerm=123456789");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().HaveCount(1);
            result.Receptions.Data.First().Patient.IdentityCard.Should().Be("123456789");
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithSearchByCode_ReturnsCorrectResult()
        {
            // Act - Search by patient code
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10&searchTerm=BN002");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().HaveCount(1);
            result.Receptions.Data.First().Patient.Code.Should().Be("BN002");
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithNoMatchingSearch_ReturnsEmptyResult()
        {
            // Act - Search with term that doesn't match any patient
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10&searchTerm=NonExistentTerm");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithPagination_ReturnsCorrectPage()
        {
            // Act - Request second page with page size 1
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=2&pageSize=1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().HaveCount(1); // Only one item per page
            result.Receptions.PageIndex.Should().Be(2);
            result.Receptions.PageSize.Should().Be(1);
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithInvalidPagination_ReturnsBadRequest()
        {
            // Act - Invalid page index (0 or negative)
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=0&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WithUnauthorizedRequest_ReturnsUnauthorized()
        {
            // Arrange - Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetRecentReceptionsWithPatients_WhenGrpcClientReturnsEmpty_ReturnsEmptyResult()
        {
            // Arrange - Mock empty response from gRPC client
            _patientGrpcClientMock
                .ListPatientsByIdsAndSearchAsync(Arg.Any<List<int>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new List<PatientSummaryDTO>());

            // Act
            var response = await _client.GetAsync("/receptions/recent-with-patients?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetRecentReceptionsWithPatientsResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result.Receptions.Data.Should().BeEmpty();
        }
    }
}