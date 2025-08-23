using BuildingBlocks.Strings.Enums;
using FluentAssertions;
using Inventory.Application.Medicines.Commands.CreateMedicine;
using Inventory.Application.Medicines.Commands.UpdateMedicine;
using Inventory.Application.Medicines.Commands.DeleteMedicine;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class MedicineValidationTests : BaseFunctionalTest
    {
        public MedicineValidationTests(FunctionalTestWebAppFactory factory)
            : base(factory) { }

        [Fact]
        public async Task CreateMedicine_WithEmptyCode_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "",
                MedicineName: "Valid Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: 1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateMedicine_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "MED002",
                MedicineName: "",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: 1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateMedicine_WithInvalidVaccineTypeId_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "MED003",
                MedicineName: "Valid Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: -1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateMedicine_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "MED004",
                MedicineName: "Valid Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: 1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateMedicine_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateMedicineCommand(
                MedicineCode: "MED005",
                MedicineName: "Unauthorized Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: 1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateMedicine_WithValidData_ReturnsOk()
        {
            // Arrange - First create a medicine
            var createCommand = new CreateMedicineCommand(
                MedicineCode: "MED006",
                MedicineName: "Medicine to Update",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: 1
            );

            var createResponse = await _client.PostAsJsonAsync("/medicines", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Get created medicine ID from response
            var locationHeader = createResponse.Headers.Location?.ToString();
            var medicineId = int.Parse(locationHeader?.Split('/').Last() ?? "1");

            // Act - Update the medicine
            var updateCommand = new UpdateMedicineCommand(
                Id: medicineId,
                MedicineCode: "MED006_UPDATED",
                MedicineName: "Updated Medicine Name",
                Unit: "Vial",
                IsRequiredTestingBeforeUse: true,
                ActiveIngredient: "Updated Ingredient",
                UsageInstructions: "Updated instructions",
                Concentration: "200mg",
                Indications: "Updated indications",
                MedicineClassification: "Updated classification",
                RouteOfAdministration: RouteOfAdministration.SC,
                NationalMedicineCode: "54321",
                Description: "Updated description",
                Note: "Updated note",
                RegistrationNumber: "UPDATED123",
                VaccineTypeId: 1,
                IsSuspended: false,
                IsCancelled: false
            );

            var updateResponse = await _client.PutAsJsonAsync(
                $"/medicines/{medicineId}",
                updateCommand
            );

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteMedicine_WithValidId_ReturnsNoContent()
        {
            // Arrange - First create a medicine
            var createCommand = new CreateMedicineCommand(
                MedicineCode: "MED007",
                MedicineName: "Medicine to Delete",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Ingredient",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: RouteOfAdministration.IM,
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                VaccineTypeId: 1
            );

            var createResponse = await _client.PostAsJsonAsync("/medicines", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Get created medicine ID
            var locationHeader = createResponse.Headers.Location?.ToString();
            var medicineId = int.Parse(locationHeader?.Split('/').Last() ?? "1");

            // Act - Delete the medicine
            var deleteResponse = await _client.DeleteAsync($"/medicines/{medicineId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
