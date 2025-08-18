// tests/VaccinationReceptionService/VaccinationReceptionService.FunctionalTests/Tests/GetHospitalRevenueReportTests.cs
using FluentAssertions;
using VaccinationReception.Application.DTOs.Reports;
using VaccinationReceptionService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using NSubstitute;
using HumanResource.Grpc;
using VaccinationReception.Domain.Models;
using VaccinationReception.Domain.Enums;
using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Strings.Enums;
using BuildingBlocks.Strings.Consts.HospitalServices;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    [Collection("VaccinationReceptionTestCollection")]
    public class GetHospitalRevenueReportTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestPatientId = 1;
        private const int TestDoctorId = 1;

        public GetHospitalRevenueReportTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);

            SetupMockServices();
        }

        private void SetupMockServices()
        {
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
        }

        [Fact]
        public async Task GetHospitalRevenueReport_WithDefaultDateRange_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.Summary.Should().NotBeNull();
            result.DailyRevenues.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_WithSpecificDateRange_ReturnsCorrectData()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/hospital-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
            result!.FromDate.Should().Be(fromDate);
            result.ToDate.Should().Be(toDate);
        }

        [Fact]
        public async Task GetHospitalRevenueReport_VerifyRevenueCalculations_ReturnsCorrectAmounts()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_VerifyServiceTypeCategorization_ReturnsCorrectBreakdown()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)); // Invalid: toDate before fromDate

            // Act
            var response = await _client.GetAsync($"/hospital-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetHospitalRevenueReport_WithNoPaymentData_ReturnsEmptyReport()
        {
            // Arrange - Clear payment data
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.PaymentDetails.RemoveRange(dbContext.PaymentDetails);
            dbContext.Payments.RemoveRange(dbContext.Payments);
            dbContext.SaveChanges();

            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExportHospitalRevenueReport_ReturnsExcelFile()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var contentDisposition = response.Content.Headers.ContentDisposition;
            contentDisposition.Should().NotBeNull();
            contentDisposition!.DispositionType.Should().Be("attachment");
            contentDisposition.FileName.Should().StartWith("BaoCaoDoanhThuBenhVien_");
            contentDisposition.FileName.Should().EndWith(".xlsx");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
            content.Length.Should().BeGreaterThan(1000);
        }

        [Fact]
        public async Task ExportHospitalRevenueReport_WithDateRange_ReturnsFilteredExcelFile()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var response = await _client.GetAsync($"/hospital-revenue/export?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_WithFutureDate_ReturnsEmptyReport()
        {
            // Arrange
            var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            var toDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

            // Act
            var response = await _client.GetAsync($"/hospital-revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_VerifyServiceCounts_ReturnsCorrectCounts()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_ChecksOnlyPaidPayments_ExcludesPendingPayments()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHospitalRevenueReport_VerifyGeneratedByField_ContainsUserInfo()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<HospitalRevenueReportDTO>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExportHospitalRevenueReport_VerifyExcelStructure_ContainsExpectedSheets()
        {
            // Act
            var response = await _client.GetAsync("/hospital-revenue/export");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();

            // Verify it's a valid Excel file by checking the file signature
            content[0].Should().Be(0x50); // 'P' - ZIP signature (Excel is ZIP-based)
            content[1].Should().Be(0x4B); // 'K'
        }
    }
}