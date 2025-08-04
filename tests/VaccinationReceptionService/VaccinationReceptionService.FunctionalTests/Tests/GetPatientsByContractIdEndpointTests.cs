using BuildingBlocks.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Domain.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class GetPatientsByContractIdEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestContractId = 100;
        private const int TestPatientId = 200;

        public GetPatientsByContractIdEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
        }

        private void SeedData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.ContractPatientVaccinations.RemoveRange(dbContext.ContractPatientVaccinations);
            dbContext.Contracts.RemoveRange(dbContext.Contracts);
            dbContext.SaveChanges();

            dbContext.Contracts.Add(new Contract
            {
                Id = TestContractId,
                ContractCode = "C001",
                ContractNumber = 1,
                ContractName = "Test Contract 1",
                CompanyName = "Test Company",
                UnitName = "Test Unit",
                ContractDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow,
                ContractValue = 1000,
                AdvanceAmount = 100,
                ActualAmount = 900,
                Description = "Test contract for today",
                FileContractId = Guid.NewGuid(),
                FileContractName = "contract.pdf",
                FileVaccinationEnrollmentId = Guid.NewGuid(),
                FileVaccinationEnrollmentName = "enroll.pdf",
                ExpectedPatientCount = 10,
                Status = ContractStatus.Active,
                IsCancelled = false,
                IsSuspended = false
            });
            dbContext.SaveChanges();

            dbContext.ContractPatientVaccinations.Add(new ContractPatientVaccination
            {
                Id = 1,
                ContractId = TestContractId,
                PatientId = TestPatientId,
                IsCancelled = false,
                IsSuspended = false
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetPatientsByContractId_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync($"/contracts/{TestContractId}/patients?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPatientsByContractId_WithInvalidContractId_ReturnsBadRequest()
        {
            // Arrange
            var invalidContractId = 0;

            // Act
            var response = await _client.GetAsync($"/contracts/{invalidContractId}/patients?pageIndex=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetPatientsByContractId_ContractNotFound_ReturnsNotFound()
        {
            SeedData();
            var notFoundContractId = 99999;

            var response = await _client.GetAsync($"/contracts/{notFoundContractId}/patients?pageIndex=1&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPatientsByContractId_WithValidData_ReturnsOkAndPatients()
        {
            SeedData();
            var patientId = 1;
            var grpcResponse = new FilteredPatientsResponse
            {
                Data = {
                    new PatientSummaryModel
                    {
                        Id = patientId,
                        Code = "BN001",
                        Name = "Nguyen Van A",
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

            var response = await _client.GetAsync($"/contracts/{TestContractId}/patients?pageIndex=1&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<PatientSummaryDTO>>();
            result.Should().NotBeNull();
        }
    }
}
