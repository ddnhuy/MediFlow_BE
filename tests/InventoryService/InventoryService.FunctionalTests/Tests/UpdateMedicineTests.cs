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
                VaccineTypeId: 1,
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicines/{999}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateMedicine_WithDuplicateMedicineCode_ReturnsBadRequest()
        {
            // Arrange: Create two medicines with different codes
            var createCommand1 = new UpdateMedicineCommand(
                Id: 0, // ID will be ignored for creation, just for structure
                MedicineCode: "CODE_A",
                MedicineName: "Medicine A",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient A",
                UsageInstructions: "Use as directed",
                Concentration: "100mg",
                Indications: "Indication A",
                MedicineClassification: "Class A",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "NMC_A",
                Description: "Description A",
                Note: "Note A",
                RegistrationNumber: "REG_A",
                VaccineTypeId: 1,
                IsSuspended: false,
                IsCancelled: false
            );
            var createCommand2 = new UpdateMedicineCommand(
                Id: 0,
                MedicineCode: "CODE_B",
                MedicineName: "Medicine B",
                Unit: "Capsule",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient B",
                UsageInstructions: "Use as directed",
                Concentration: "200mg",
                Indications: "Indication B",
                MedicineClassification: "Class B",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "NMC_B",
                Description: "Description B",
                Note: "Note B",
                RegistrationNumber: "REG_B",
                VaccineTypeId: 2,
                IsSuspended: false,
                IsCancelled: false
            );

            // Create both medicines (assuming POST /medicines returns created entity with ID)
            var response1 = await _client.PostAsJsonAsync("/medicines", createCommand1);
            var created1 = await response1.Content.ReadFromJsonAsync<Medicine>();
            var response2 = await _client.PostAsJsonAsync("/medicines", createCommand2);
            var created2 = await response2.Content.ReadFromJsonAsync<Medicine>();

            // Act: Try to update the second medicine to have the same code as the first
            var updateCommand = new UpdateMedicineCommand(
                Id: created2.Id,
                MedicineCode: " CODE_A ", // Duplicate code
                MedicineName: "Medicine B Updated",
                Unit: "Capsule",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient B",
                UsageInstructions: "Use as directed",
                Concentration: "200mg",
                Indications: "Indication B",
                MedicineClassification: "Class B",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "NMC_B",
                Description: "Description B",
                Note: "Note B",
                RegistrationNumber: "REG_B",
                VaccineTypeId: 2,
                IsSuspended: false,
                IsCancelled: false
            );

            var updateResponse = await _client.PutAsJsonAsync($"/medicines/{created2.Id}", updateCommand);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
