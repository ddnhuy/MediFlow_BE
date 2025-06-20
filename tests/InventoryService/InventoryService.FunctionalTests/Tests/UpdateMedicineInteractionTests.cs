using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Data;
using Inventory.Application.Medicines.Commands.UpdateMedicineInteraction;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    [Collection("InventoryTestCollection")]
    public class UpdateMedicineInteractionTests : BaseFunctionalTest
    {
        public UpdateMedicineInteractionTests(FunctionalTestWebAppFactory factory) : base(factory) {}

        [Fact]
        public async Task Update_WithValidData_ReturnsOk()
        {
            // Correct initialization for a record with positional parameters
            var command = new UpdateMedicineInteractionCommand(
                Id: 1, // Match your seeded ID
                MedicineId1: 1,
                HarmfulEffects: "Updated harmful effects_update",
                MedicineId2: 2,
                PreventiveActions: "Updated preventive actions",
                Mechanism: "Updated mechanism",
                Notes: "Updated notes",
                ReferenceInfo: "Updated reference",
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/{command.Id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateMedicineInteractionResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Update_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new UpdateMedicineInteractionCommand(
                Id: 1, // Match your seeded ID
                MedicineId2: 2,
                MedicineId1: 1,
                PreventiveActions: "Updated preventive actions",
                HarmfulEffects: "Updated harmful effects_update",
                Mechanism: "Updated mechanism",
                Notes: "Updated notes",
                IsSuspended: false,
                IsCancelled: false,
                ReferenceInfo: "Updated reference"
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/{command.Id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateMedicineInteractionCommand(
                Id: 1, // Match your seeded ID
                MedicineId1: 1,
                MedicineId2: 2,
                HarmfulEffects: "Updated harmful effects_update",
                Mechanism: "Updated mechanism",
                PreventiveActions: "Updated preventive actions",
                ReferenceInfo: "Updated reference",
                Notes: "Updated notes",
                IsSuspended: false,
                IsCancelled: false
            );

            // Act - Using different ID in route
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/1001", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // Case no interaction found with the given ID
        [Fact]
        public async Task Update_WhenInteractionNotFound_ReturnsNotFound()
        {
            // Arrange
            var command = new UpdateMedicineInteractionCommand(
                Id: 9999, // Non-existent ID
                MedicineId1: 1,
                MedicineId2: 2,
                HarmfulEffects: "Updated harmful effects_update",
                Mechanism: "Updated mechanism",
                PreventiveActions: "Updated preventive actions",
                ReferenceInfo: "Updated reference",
                Notes: "Updated notes",
                IsSuspended: false,
                IsCancelled: false
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/{command.Id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Case first medicine not found
        [Fact]
        public async Task Update_WhenFirstMedicineNotFound_ReturnsNotFound()
        {
            // Arrange
            var command = new UpdateMedicineInteractionCommand(
                Id: 1, // Match your seeded ID
                MedicineId1: 9999, // Non-existent first medicine ID
                MedicineId2: 2,
                HarmfulEffects: "Updated harmful effects_update",
                Mechanism: "Updated mechanism",
                PreventiveActions: "Updated preventive actions",
                ReferenceInfo: "Updated reference",
                Notes: "Updated notes",
                IsSuspended: false,
                IsCancelled: false
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/{command.Id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Case second medicine not found
        [Fact]
        public async Task Update_WhenSecondMedicineNotFound_ReturnsNotFound()
        {
            // Arrange
            var command = new UpdateMedicineInteractionCommand(
                Id: 1, // Match your seeded ID
                MedicineId1: 1,
                MedicineId2: 9999, // Non-existent second medicine ID
                HarmfulEffects: "Updated harmful effects_update",
                Mechanism: "Updated mechanism",
                PreventiveActions: "Updated preventive actions",
                ReferenceInfo: "Updated reference",
                Notes: "Updated notes",
                IsSuspended: false,
                IsCancelled: false
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/{command.Id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // Case a different interaction with the same medicines exist
        [Fact]

        public async Task Update_WhenDifferentInteractionExists_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateMedicineInteractionCommand(
                Id: 2, // Match your seeded ID
                MedicineId1: 1, // Assuming this interaction already exists with different ID
                MedicineId2: 2,
                HarmfulEffects: "Updated harmful effects_update",
                Mechanism: "Updated mechanism",
                PreventiveActions: "Updated preventive actions",
                ReferenceInfo: "Updated reference",
                Notes: "Updated notes",
                IsSuspended: false,
                IsCancelled: false
            );
            // Act
            var response = await _client.PutAsJsonAsync($"/medicine-interactions/{command.Id}", command);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}