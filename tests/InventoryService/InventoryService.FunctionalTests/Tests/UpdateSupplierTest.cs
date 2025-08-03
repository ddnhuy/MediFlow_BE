using FluentAssertions;
using Inventory.Application.Suppliers.Commands.CreateSupplier;
using Inventory.Application.Suppliers.Commands.UpdateSupplier;
using InventoryService.FunctionalTests.Abstractions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class UpdateSupplierTests : BaseFunctionalTest
    {
        public UpdateSupplierTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Update_WithValidData_ReturnsOk()
        {
            // Arrange
            var command = new UpdateSupplierCommand(
                Id: 1, 
                SupplierName: "MedPharm Supply Co. Updated",
                Phone: "0981995925",
                Fax: "555-123-9998",
                Email: "updated@medpharm.example",
                TaxCode: "MP12345-U",
                Address: "123 Medical Plaza, Suite 200",
                ContactPerson: "Michael Lewis Jr.",
                Director: "Sarah Johnson-Smith",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<UpdateSupplierContractRequest>()
                {
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract1.pdf"),
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract2.pdf")
                },
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/suppliers/{command.Id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<UpdateSupplierResult>();
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Update_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new UpdateSupplierCommand(
                Id: 1,
                //SupplierCode: "SUP001",
                SupplierName: "MedPharm Supply Co. Updated",
                Phone: "0981995925",
                Fax: "555-123-9998",
                Email: "updated@medpharm.example",
                TaxCode: "MP12345-U",
                Address: "123 Medical Plaza, Suite 200",
                ContactPerson: "Michael Lewis Jr.",
                Director: "Sarah Johnson-Smith",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<UpdateSupplierContractRequest>()
                {
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract1.pdf"),
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract2.pdf")
                },
                IsSuspended: false,
                IsCancelled: false
            );

            var response = await _client.PutAsJsonAsync($"/suppliers/{command.Id}", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_WithIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateSupplierCommand(
                Id: 2, // ID in command
                SupplierName: "Healthcare Distributors Inc. Updated",
                Phone: "0981995925",
                Fax: "555-987-1112",
                Email: "updated@healthdist.example",
                TaxCode: "HD67890-U",
                Address: "456 Hospital Drive, Suite 300",
                ContactPerson: "Emma Wilson-Lee",
                Director: "Robert Chen Jr.",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<UpdateSupplierContractRequest>()
                {
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract1.pdf"),
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract2.pdf")
                },
                IsSuspended: false,
                IsCancelled: false
            );

            // Act - Using different ID in route
            var response = await _client.PutAsJsonAsync("/suppliers/1", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var command = new UpdateSupplierCommand(
                Id: 999, // Non-existent ID
                //SupplierCode: "SUP999",
                SupplierName: "Non-Existent Supplier",
                Phone: "0981995925",
                Fax: "555-999-9998",
                Email: "nonexistent@example.com",
                TaxCode: "NE12345",
                Address: "999 Nowhere Street",
                ContactPerson: "No One",
                Director: "Not Available",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<UpdateSupplierContractRequest>()
                {
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract1.pdf"),
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "contract2.pdf")
                },
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/suppliers/{command.Id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithMissingRequiredFields_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateSupplierCommand(
                Id: 1,
                SupplierName: "", // Empty required field
                Phone: "0981995925",
                Fax: "555-123-9998",
                Email: "updated@medpharm.example",
                TaxCode: "MP12345-U",
                Address: "123 Medical Plaza, Suite 200",
                ContactPerson: "Michael Lewis Jr.",
                Director: "Sarah Johnson-Smith",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<UpdateSupplierContractRequest>(),
                IsSuspended: false,
                IsCancelled: false
            );

            // Act
            var response = await _client.PutAsJsonAsync($"/suppliers/{command.Id}", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
