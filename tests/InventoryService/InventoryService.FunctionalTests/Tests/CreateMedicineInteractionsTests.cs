using FluentAssertions;
using Inventory.API.Endpoints;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class CreateMedicineInteractionTests : BaseFunctionalTest
    {
        public CreateMedicineInteractionTests(FunctionalTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 1,
                MedicineId2: 3,
                HarmfulEffects: "May cause drowsiness",
                Mechanism: "CNS depression",
                PreventiveActions: "Do not drive after taking",
                ReferenceInfo: "Medical journal reference",
                Notes: "Additional notes"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory/medicine-interactions", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CreateMedicineInteractionResponse>();
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Create_WithSameMedicines_ReturnsBadRequest()
        {
            // Arrange
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 1,
                MedicineId2: 1,
                HarmfulEffects: "Test harmful effects",
                Mechanism: "Test mechanism",
                PreventiveActions: "Test preventive actions",
                ReferenceInfo: "Test reference",
                Notes: "Test notes"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory/medicine-interactions", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_WithExistingInteraction_ReturnsBadRequest()
        {
            // Arrange - existing interaction between medicines 1 and 2
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 1,
                MedicineId2: 2,
                HarmfulEffects: "Test harmful effects",
                Mechanism: "Test mechanism",
                PreventiveActions: "Test preventive actions",
                ReferenceInfo: "Test reference",
                Notes: "Test notes"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/inventory/medicine-interactions", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }
    }
}