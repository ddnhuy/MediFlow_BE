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
    public class UpdateExaminationServiceEndpointTests : BaseFunctionalTest
    {
        public UpdateExaminationServiceEndpointTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateExaminationService_WhenCalled_ReturnsSuccess()
        {
            // Arrange
            var serviceId = 5; // Assuming there's a seeded examination service with ID 5
            var command = new UpdateExaminationServiceCommand(
                ServiceId: serviceId,
                ServiceCode: "EXAM001_UPDATED",
                ServiceName: "Blood Test Updated",
                UnitPrice: 200000,
                DepartmentId: 2,
                ExaminationService: BuildingBlocks.Strings.Enums.ExaminationService.Blood,
                ServiceTestParameters: new List<ServiceTestParameterDto>
                {
                    new ServiceTestParameterDto(
                        ParameterName: "Hemoglobin Updated",
                        Unit: "g/dL",
                        StandardValue: "13.5-17.5",
                        EquipmentName: "Sysmex XN-2000",
                        SpecimenType: "Blood"
                    ),
                    new ServiceTestParameterDto(
                        ParameterName: "Platelet Count",
                        Unit: "10^9/L",
                        StandardValue: "150-450",
                        EquipmentName: "Sysmex XN-2000",
                        SpecimenType: "Blood"
                    )
                }
            );

            var jsonContent = JsonSerializer.Serialize(command);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/services/examination/{serviceId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadFromJsonAsync<UpdateExaminationServiceResponse>();

            responseContent.Should().NotBeNull();
            responseContent!.ServiceId.Should().Be(serviceId);
        }

        [Fact]
        public async Task UpdateExaminationService_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            // BaseFunctionalTest adds auth by default, so we create a new client without it.
            _client.DefaultRequestHeaders.Remove("Authorization");
            var serviceId = 1;

            var command = new UpdateExaminationServiceCommand(
                ServiceId: serviceId,
                ServiceCode: "EXAM002_UPDATED",
                ServiceName: "Anti-HBs Test Updated",
                UnitPrice: 250000,
                DepartmentId: 2,
                ExaminationService: BuildingBlocks.Strings.Enums.ExaminationService.Anti_HBs,
                ServiceTestParameters: new List<ServiceTestParameterDto>
                {
                    new ServiceTestParameterDto(
                        ParameterName: "Anti-HBs Updated",
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
            var response = await _client.PutAsync($"/services/examination/{serviceId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task UpdateExaminationService_WithInvalidId_ReturnsBadRequest(int invalidId)
        {
            // Arrange
            var command = new UpdateExaminationServiceCommand(
                ServiceId: invalidId,
                ServiceCode: "EXAM_INVALID",
                ServiceName: "Invalid Service",
                UnitPrice: 100000,
                DepartmentId: 2,
                ExaminationService: BuildingBlocks.Strings.Enums.ExaminationService.Blood,
                ServiceTestParameters: new List<ServiceTestParameterDto>
                {
                    new ServiceTestParameterDto(
                        ParameterName: "Test Parameter",
                        Unit: "unit",
                        StandardValue: "value",
                        EquipmentName: "equipment",
                        SpecimenType: "specimen"
                    )
                }
            );

            var jsonContent = JsonSerializer.Serialize(command);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/services/examination/{invalidId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}