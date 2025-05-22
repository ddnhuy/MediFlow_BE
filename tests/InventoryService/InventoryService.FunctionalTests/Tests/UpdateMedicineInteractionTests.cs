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
            var response = await _client.PutAsJsonAsync($"/inventory/medicine-interactions/{command.Id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateMedicineInteractionResponse>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
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
            var response = await _client.PutAsJsonAsync($"/inventory/medicine-interactions/1001", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}