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
        public async Task Create_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "TEST001",
                MedicineName: "Test Medicine",
                Unit: "Tablet",
                Manufacturer: "Test Manufacturer",
                ActiveIngredient: "Test Compound 50mg",
                UsageInstructions: "Take once daily",
                Concentration: "50mg",
                Indications: "For testing purposes",
                MedicineClassification: "Test Classification",
                RouteOfAdministration: "Oral",
                NationalMedicineCode: "12345-6789-01",
                Description: "Test medicine description",
                Note: "Created in test",
                RegistrationNumber: "REG-TEST-001",
                MedicineTypeId: 1,
                VaccineTypeId: 1
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_WithMissingRequiredFields_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateMedicineCommand(
                MedicineCode: "",
                MedicineName: "",
                Unit: "",
                Manufacturer: "Test Manufacturer",
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
            var response = await _client.PostAsJsonAsync("/inventory/medicines", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }
    }
}
