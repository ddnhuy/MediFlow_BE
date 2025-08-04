using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineStockStatus;
using BuildingBlocks.Strings.Enums;
using Docker.DotNet.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VaccinationReception.API.EndPoints.VaccinationReceptionContractEndpoints;
using VaccinationReception.Application.DTOs.ExcelDTOs;
using VaccinationReception.Application.DTOs.ReceptionVaccinationContractDTOs;
using VaccinationReception.Application.DTOs.VaccinationDTOs;
using VaccinationReception.Application.Services.ExcelServices;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReceptionService.FunctionalTests.Tests
{
    public class RegisterContractEndpointTests : BaseFunctionalTest
    {
        private readonly string _testToken;
        private readonly IExcelDataReaderService _excelServiceMock;
        private readonly FunctionalTestWebAppFactory _factory;
        private const int TestReceptionId = 1;
        private const int TestVaccineId = 1;
        private const int TestDoctorId = 1;
        private const int TestServiceTypeId = 1;

        public RegisterContractEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
            _factory = factory;
            _testToken = TokenHelper.GenerateTestToken();
            // Mock ExcelDataReaderService
            _excelServiceMock = Substitute.For<IExcelDataReaderService>();
        }

        [Fact]
        public async Task RegisterContract_Unauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var request = new RegisterContractRequest(
                Contract: new ContractDTO
                {
                    ContractCode = "CT-001",
                    ContractNumber = 1,
                    ContractName = "Hợp đồng tiêm chủng công ty ABC",
                    CompanyName = "Công ty ABC",
                    UnitName = "Đơn vị A",
                    ContractDate = DateTime.UtcNow,
                    ExpectedDate = DateTime.UtcNow.AddMonths(1),
                    Description = "Hợp đồng tiêm chủng cho nhân viên công ty ABC",
                    FileContractId = Guid.NewGuid(),
                    FileVaccinationEnrollmentId = Guid.NewGuid(),
                    FileContractName = "contract.pdf",
                    FileVaccinationEnrollmentName = "enrollment.xlsx"
                },
                VaccinationEnrollmentDownloadUrl: "http://example.com/file.xlsx"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/contracts/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RegisterContract_BadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _testToken);
            var request = new RegisterContractRequest(
                Contract: new ContractDTO
                {
                    ContractCode = "CT-001",
                    ContractNumber = 1,
                    ContractName = "Hợp đồng tiêm chủng công ty ABC",
                    CompanyName = "Công ty ABC",
                    UnitName = "Đơn vị A",
                    ContractDate = DateTime.UtcNow,
                    ExpectedDate = DateTime.UtcNow.AddMonths(1),
                    Description = "Hợp đồng tiêm chủng cho nhân viên công ty ABC",
                    FileContractId = Guid.NewGuid(),
                    FileVaccinationEnrollmentId = Guid.NewGuid(),
                    FileContractName = "contract.pdf",
                    FileVaccinationEnrollmentName = "enrollment.xlsx"
                },
                VaccinationEnrollmentDownloadUrl: ""
            );

            var response = await _client.PostAsJsonAsync("/contracts/register", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}