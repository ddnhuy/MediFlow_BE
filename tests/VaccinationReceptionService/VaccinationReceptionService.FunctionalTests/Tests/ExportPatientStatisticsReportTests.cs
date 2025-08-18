using FluentAssertions;
using VaccinationReceptionService.FunctionalTests.Abstractions;
using System.Net;
using Xunit;
using NSubstitute;
using HumanResource.Grpc;
using VaccinationReception.Domain.Models;
using VaccinationReception.Application.DTOs.PatientDTOs;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    [Collection("VaccinationReceptionTestCollection")]
    public class ExportPatientStatisticsReportTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;

        public ExportPatientStatisticsReportTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SetupMockServices();
            SeedTestData();
        }

        private void SetupMockServices()
        {
            // Setup ApplicationUserProtoService mock
            // Setup ApplicationUserProtoService mock
            var userResponse = new ApplicationUserDetailModel
            {
                Id = 1,
                Name = "Admin"
            };

            var userAsyncUnaryCall = new AsyncUnaryCall<ApplicationUserDetailModel>(
                Task.FromResult(userResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });

            _factory.ApplicationUserProtoMock?
                .GetApplicationUserAsync(Arg.Any<GetApplicationUserRequest>(), Arg.Any<Metadata>(), null, default)
                .Returns(userAsyncUnaryCall);

            // Setup PatientGrpcClient mock
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
        }

        private void SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clear existing data
            dbContext.Receptions.RemoveRange(dbContext.Receptions);
            dbContext.SaveChanges();

            // Seed test receptions
            var receptions = new List<Reception>
            {
                new Reception
                {
                    Id = 1,
                    ServiceTypeId = 1,
                    PatientId = 1,
                    ReceptionDate = DateTime.UtcNow.AddDays(-5),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                },
                new Reception
                {
                    Id = 2,
                    ServiceTypeId = 1,
                    PatientId = 2,
                    ReceptionDate = DateTime.UtcNow.AddDays(-3),
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    CreatedBy = 1,
                    LastUpdatedAt = DateTime.UtcNow,
                    LastUpdatedBy = 1
                }
            };
            dbContext.Receptions.AddRange(receptions);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task ExportPatientStatisticsReport_WithValidRequest_ReturnsExcelFile()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/patient-statistics/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var contentDisposition = response.Content.Headers.ContentDisposition;
            contentDisposition.Should().NotBeNull();
            contentDisposition!.DispositionType.Should().Be("attachment");
            contentDisposition.FileName.Should().StartWith("BaoCaoThongKeBenhNhan_");
            contentDisposition.FileName.Should().EndWith(".xlsx");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
            content.Length.Should().BeGreaterThan(1000);

            // Verify Excel file signature
            content[0].Should().Be(0x50); // 'P'
            content[1].Should().Be(0x4B); // 'K'
        }

        [Fact]
        public async Task ExportPatientStatisticsReport_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)); // Invalid: toDate before fromDate

            // Act
            var response = await _client.GetAsync($"/patient-statistics/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ExportPatientStatisticsReport_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Arrange - Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.GetAsync("/patient-statistics/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}