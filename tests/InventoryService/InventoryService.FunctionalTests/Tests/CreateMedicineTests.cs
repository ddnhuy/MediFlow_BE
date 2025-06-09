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
    }
}
