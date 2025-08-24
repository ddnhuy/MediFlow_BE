using FluentAssertions;
using Inventory.Application.Suppliers.Commands.CreateSupplier;
using Inventory.Application.Suppliers.Commands.UpdateSupplier;
using Inventory.Application.Suppliers.Commands.DeleteSupplier;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class SupplierManagementTests : BaseFunctionalTest
    {
        public SupplierManagementTests(FunctionalTestWebAppFactory factory)
            : base(factory) { }

        [Fact]
        public async Task CreateSupplier_WithValidData_ReturnsCreated()
        {
            // Arrange
            var command = new CreateSupplierCommand(
                SupplierName: "Test Supplier Co.",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "test@supplier.com",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateSupplier_WithEmptyName_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateSupplierCommand(
                SupplierName: "",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "test@supplier.com",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateSupplier_WithInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateSupplierCommand(
                SupplierName: "Test Supplier Co.",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "invalid-email",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateSupplier_WithPastExpiredDate_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateSupplierCommand(
                SupplierName: "Test Supplier Co.",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "test@supplier.com",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateSupplier_WithValidData_ReturnsOk()
        {
            // Arrange - First create a supplier
            var createCommand = new CreateSupplierCommand(
                SupplierName: "Supplier to Update",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "update@supplier.com",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            var createResponse = await _client.PostAsJsonAsync("/suppliers", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Get created supplier ID from response
            var locationHeader = createResponse.Headers.Location?.ToString();
            var supplierId = int.Parse(locationHeader?.Split('/').Last() ?? "1");

            // Act - Update the supplier
            var updateCommand = new UpdateSupplierCommand(
                Id: supplierId,
                SupplierName: "Updated Supplier Name",
                Phone: "0987654321",
                Fax: "0987654320",
                Email: "updated@supplier.com",
                TaxCode: "UPDATED123456",
                Address: "456 Updated Street, Updated City",
                ContactPerson: "John Updated",
                Director: "Jane Updated",
                IsSuspended: false,
                IsCancelled: false,
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                Contracts: new List<UpdateSupplierContractRequest>
                {
                    new UpdateSupplierContractRequest(Guid.NewGuid(), "UpdatedContract1.pdf")
                }
            );

            var updateResponse = await _client.PutAsJsonAsync(
                $"/suppliers/{supplierId}",
                updateCommand
            );

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteSupplier_WithValidId_ReturnsOk()
        {
            // Arrange - First create a supplier
            var createCommand = new CreateSupplierCommand(
                SupplierName: "Supplier to Delete",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "delete@supplier.com",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            var createResponse = await _client.PostAsJsonAsync("/suppliers", createCommand);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Get created supplier ID
            var locationHeader = createResponse.Headers.Location?.ToString();
            var supplierId = int.Parse(locationHeader?.Split('/').Last() ?? "1");

            // Act - Delete the supplier
            var deleteResponse = await _client.DeleteAsync($"/suppliers/{supplierId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateSupplier_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateSupplierCommand(
                SupplierName: "Unauthorized Supplier",
                Address: "123 Test Street, Test City",
                Phone: "0123456789",
                Fax: "0123456788",
                Email: "unauthorized@supplier.com",
                TaxCode: "TAX123456",
                Director: "John Smith",
                ContactPerson: "Jane Doe",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "Contract1.pdf")
                }
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
