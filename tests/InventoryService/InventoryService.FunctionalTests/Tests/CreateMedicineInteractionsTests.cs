using FluentAssertions;
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
            var response = await _client.PostAsJsonAsync("/medicine-interactions", command);

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
            var response = await _client.PostAsJsonAsync("/medicine-interactions", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsCreated()
        {
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 1,
                MedicineId2: 3,
                HarmfulEffects: "None",
                Mechanism: "None",
                PreventiveActions: "None",
                ReferenceInfo: "None",
                Notes: "None"
            );

            var response = await _client.PostAsJsonAsync("/medicine-interactions", command);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 1,
                MedicineId2: 3,
                HarmfulEffects: "None",
                Mechanism: "None",
                PreventiveActions: "None",
                ReferenceInfo: "None",
                Notes: "None"
            );

            var response = await _client.PostAsJsonAsync("/medicine-interactions", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // Case first medicine does not exist
        [Fact]
        public async Task Create_WhenFirstMedicineDoesNotExist_ReturnsNotFound()
        {
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 999, // Assuming 999 does not exist
                MedicineId2: 3,
                HarmfulEffects: "None",
                Mechanism: "None",
                PreventiveActions: "None",
                ReferenceInfo: "None",
                Notes: "None"
            );
            var response = await _client.PostAsJsonAsync("/medicine-interactions", command);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Case second medicine does not exist
        [Fact]
        public async Task Create_WhenSecondMedicineDoesNotExist_ReturnsNotFound()
        {
            var command = new Application.Medicines.Commands.CreateMedicineInteraction.CreateMedicineInteractionCommand(
                MedicineId1: 1,
                MedicineId2: 999, // Assuming 999 does not exist
                HarmfulEffects: "None",
                Mechanism: "None",
                PreventiveActions: "None",
                ReferenceInfo: "None",
                Notes: "None"
            );
            var response = await _client.PostAsJsonAsync("/medicine-interactions", command);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}