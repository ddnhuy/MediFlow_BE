using HospitalService.API.Endpoints;
using HospitalService.Application.Services.HospitalServices.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HospitalService.FunctionalTests.Tests
{
    public class CreateExaminationServiceEndpointTests : BaseFunctionalTest
    {
        public CreateExaminationServiceEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateExaminationService_WhenCalled_ReturnsSuccess()
        {
            // Arrange
            var command = new CreateExaminationServiceCommand(
                ServiceCode: "EXAM001",
                ServiceName: "Blood Test",
                UnitPrice: 150000,
                DepartmentId: 2,
                ExaminationService: BuildingBlocks.Strings.Enums.ExaminationService.Blood,
                ServiceTestParameters: new List<ServiceTestParameterDto>
                {
                    new ServiceTestParameterDto(
                        ParameterName: "Hemoglobin",
                        Unit: "g/dL",
                        StandardValue: "13.5-17.5",
                        EquipmentName: "Sysmex XN-1000",
                        SpecimenType: "Blood"
                    ),
                    new ServiceTestParameterDto(
                        ParameterName: "White Blood Cell Count",
                        Unit: "10^9/L",
                        StandardValue: "4.0-11.0",
                        EquipmentName: "Sysmex XN-1000",
                        SpecimenType: "Blood"
                    )
                }
            );

            var jsonContent = JsonSerializer.Serialize(command);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/services/examination", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseContent = await response.Content.ReadFromJsonAsync<CreateExaminationServiceResponse>();

            responseContent.Should().NotBeNull();
            responseContent!.ServiceId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateExaminationService_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // BaseFunctionalTest adds auth by default, so we create a new client without it.
            _client.DefaultRequestHeaders.Remove("Authorization");

            var command = new CreateExaminationServiceCommand(
                ServiceCode: "EXAM002",
                ServiceName: "Anti-HBs Test",
                UnitPrice: 200000,
                DepartmentId: 2,
                ExaminationService: BuildingBlocks.Strings.Enums.ExaminationService.Anti_HBs,
                ServiceTestParameters: new List<ServiceTestParameterDto>
                {
                    new ServiceTestParameterDto(
                        ParameterName: "Anti-HBs",
                        Unit: "mIU/mL",
                        StandardValue: "< 10",
                        EquipmentName: "Architect i2000",
                        SpecimenType: "Blood"
                    )
                }
            );

            var jsonContent = JsonSerializer.Serialize(command);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/services/examination", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateExaminationService_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateExaminationServiceCommand(
                ServiceCode: "", // Invalid: empty service code
                ServiceName: "", // Invalid: empty service name
                UnitPrice: -100, // Invalid: negative price
                DepartmentId: 0, // Invalid: zero department ID
                ExaminationService: BuildingBlocks.Strings.Enums.ExaminationService.Blood,
                ServiceTestParameters: new List<ServiceTestParameterDto>() // Invalid: empty parameters
            );

            var jsonContent = JsonSerializer.Serialize(command);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/services/examination", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}