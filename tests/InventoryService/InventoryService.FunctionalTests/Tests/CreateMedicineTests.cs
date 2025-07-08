using FluentAssertions;
using Inventory.Application.Medicines.Commands.CreateMedicine;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class CreateMedicineTests : BaseFunctionalTest
    {
        public CreateMedicineTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_WithMissingRequiredFields_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "",
                MedicineName: "",
                Unit: "",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Test Compound",
                UsageInstructions: "Test instructions",
                Concentration: "100mg",
                Indications: "Test indications",
                MedicineClassification: "Test classification",
                RouteOfAdministration: "Oral",
                NationalMedicineCode: "12345",
                Description: "Test description",
                Note: "Test note",
                RegistrationNumber: "TEST123",
                MedicineTypeId: 0,
                VaccineTypeId: 0
            );

            // Act
            var response = await _client.PostAsJsonAsync("/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsCreated()
        {
            var command = new CreateMedicineCommand(
                MedicineCode: "MED100",
                MedicineName: "Valid Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient",
                UsageInstructions: "Use as directed",
                Concentration: "500mg",
                Indications: "Pain relief",
                MedicineClassification: "Analgesic",
                RouteOfAdministration: "Oral",
                NationalMedicineCode: "NMC100",
                Description: "Description",
                Note: "Note",
                RegistrationNumber: "REG100",
                MedicineTypeId: 1,
                VaccineTypeId: 1
            );

            var response = await _client.PostAsJsonAsync("/medicines", command);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateMedicineCommand(
                MedicineCode: "MED101",
                MedicineName: "Unauthorized Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient",
                UsageInstructions: "Use as directed",
                Concentration: "500mg",
                Indications: "Pain relief",
                MedicineClassification: "Analgesic",
                RouteOfAdministration: "Oral",
                NationalMedicineCode: "NMC101",
                Description: "Description",
                Note: "Note",
                RegistrationNumber: "REG101",
                MedicineTypeId: 1,
                VaccineTypeId: 1
            );

            var response = await _client.PostAsJsonAsync("/medicines", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_WithDuplicateMedicineCode_ReturnsBadRequest()
        {
            // Arrange: First, create a medicine with a unique code
            var command = new CreateMedicineCommand(
                MedicineCode: "DUPLICATE_CODE",
                MedicineName: "First Medicine",
                Unit: "Tablet",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient",
                UsageInstructions: "Use as directed",
                Concentration: "500mg",
                Indications: "Pain relief",
                MedicineClassification: "Analgesic",
                RouteOfAdministration: "Oral",
                NationalMedicineCode: "NMC200",
                Description: "Description",
                Note: "Note",
                RegistrationNumber: "REG200",
                MedicineTypeId: 1,
                VaccineTypeId: 1
            );
            var response1 = await _client.PostAsJsonAsync("/medicines", command);
            response1.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act: Try to create another medicine with the same code
            var duplicateCommand = new CreateMedicineCommand(
                MedicineCode: "DUPLICATE_CODE", // Same code as above
                MedicineName: "Second Medicine",
                Unit: "Capsule",
                IsRequiredTestingBeforeUse: false,
                ActiveIngredient: "Ingredient2",
                UsageInstructions: "Use as directed",
                Concentration: "250mg",
                Indications: "Fever",
                MedicineClassification: "Antipyretic",
                RouteOfAdministration: "Oral",
                NationalMedicineCode: "NMC201",
                Description: "Description2",
                Note: "Note2",
                RegistrationNumber: "REG201",
                MedicineTypeId: 2,
                VaccineTypeId: 2
            );
            var response2 = await _client.PostAsJsonAsync("/medicines", duplicateCommand);

            // Assert
            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
