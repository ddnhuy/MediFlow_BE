using BuildingBlocks.Strings.Enums;
using FluentAssertions;
using Inventory.Application.Medicines.Commands.UpdateMedicine;
using Inventory.Domain.Models;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class UpdateMedicineTests : BaseFunctionalTest
    {
        public UpdateMedicineTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task UpdateMedicine_WithValidData_ReturnsOk()
        {
            // Arrange - Use ID from seeded medicine

            var updateCommand = new UpdateMedicineCommand(
                Id: 1,
                MedicineCode: "PARA001",
                MedicineName: "Paracetamol Updated",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Acetaminophen 500mg",
                UsageInstructions: "Take as directed",
                Concentration: "500mg",
                Indications: "Pain relief",
                MedicineClassification: "Analgesic",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345-678-90",
                Description: "For pain relief",
                Note: "Updated in test",
                RegistrationNumber: "REG12345",
                MedicineTypeId: 1,
                VaccineTypeId: 1,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicines/{1}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateMedicine_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var updateCommand = new UpdateMedicineCommand(
                Id: 999,
                MedicineCode: "PARA001",
                MedicineName: "Paracetamol Updated",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Acetaminophen 500mg",
                UsageInstructions: "Take as directed",
                Concentration: "500mg",
                Indications: "Pain relief",
                MedicineClassification: "Analgesic",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345-678-90",
                Description: "For pain relief",
                Note: "Updated in test",
                RegistrationNumber: "REG12345",
                MedicineTypeId: 1,
                VaccineTypeId: 1,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicines/1", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateMedicine_WithInvalidId_ReturnsNotFound()
        {
            // Arrange

            var updateCommand = new UpdateMedicineCommand(
                Id: 999,
                MedicineCode: "PARA001",
                MedicineName: "Paracetamol Updated",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Acetaminophen 500mg",
                UsageInstructions: "Take as directed",
                Concentration: "500mg",
                Indications: "Pain relief",
                MedicineClassification: "Analgesic",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345-678-90",
                Description: "For pain relief",
                Note: "Updated in test",
                RegistrationNumber: "REG12345",
                MedicineTypeId: 1,
                VaccineTypeId: 1,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicines/{999}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
