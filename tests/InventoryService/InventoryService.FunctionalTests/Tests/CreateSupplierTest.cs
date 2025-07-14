using FluentAssertions;
using Inventory.Application.Suppliers.Commands.CreateSupplier;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class CreateSupplierTests : BaseFunctionalTest
    {
        public CreateSupplierTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task Create_WithMissingSupplierCode_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateSupplierCommand(
                SupplierCode: "", // Empty supplier code
                SupplierName: "Test Supplier",
                Address: "123 Test Street",
                Phone: "0981995925",
                Fax: "+1-212-555-0102",
                Email: "contact@testsupplier.com",
                TaxCode: "TAX12345",
                Director: "John Director",
                ContactPerson: "Jane Contact"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_WithMissingSupplierName_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateSupplierCommand(
                SupplierCode: "SUP005",
                SupplierName: "", // Empty supplier name
                Address: "123 Test Street",
                Phone: "0981995925",
                Fax: "+1-212-555-0102",
                Email: "contact@testsupplier.com",
                TaxCode: "TAX12345",
                Director: "John Director",
                ContactPerson: "Jane Contact"
            );

            // Act
            var response = await _client.PostAsJsonAsync("/suppliers", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsCreated()
        {
            var command = new CreateSupplierCommand(
                SupplierCode: "SUP100",
                SupplierName: "Valid Supplier",
                Address: "123 Valid St",
                Phone: "1234567890",
                Fax: "1234567891",
                Email: "valid@supplier.com",
                TaxCode: "TAX100",
                Director: "Director Name",
                ContactPerson: "Contact Name"
            );

            var response = await _client.PostAsJsonAsync("/suppliers", command);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateSupplierCommand(
                SupplierCode: "SUP101",
                SupplierName: "Unauthorized Supplier",
                Address: "123 Unauthorized St",
                Phone: "1234567890",
                Fax: "1234567891",
                Email: "unauth@supplier.com",
                TaxCode: "TAX101",
                Director: "Director Name",
                ContactPerson: "Contact Name"
            );

            var response = await _client.PostAsJsonAsync("/suppliers", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // Case duplicate supplier code     
        [Fact]
        public async Task Create_WithDuplicateSupplierCode_ReturnsBadRequest()
        {
            // Arrange:
            var initialCommand = new CreateSupplierCommand(
                SupplierCode: "DUP001",
                SupplierName: "Initial Supplier",
                Address: "123 Main St",
                Phone: "0123456789",
                Fax: "0123456790",
                Email: "initial@supplier.com",
                TaxCode: "TAX001",
                Director: "Initial Director",
                ContactPerson: "Initial Contact"
            );

            var response1 = await _client.PostAsJsonAsync("/suppliers", initialCommand);
            response1.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act: 
            var duplicateCommand = new CreateSupplierCommand(
                SupplierCode: "DUP001", // same code
                SupplierName: "Duplicate Supplier",
                Address: "456 Another St",
                Phone: "0987654321",
                Fax: "0987654322",
                Email: "duplicate@supplier.com",
                TaxCode: "TAX002",
                Director: "Duplicate Director",
                ContactPerson: "Duplicate Contact"
            );

            var response2 = await _client.PostAsJsonAsync("/suppliers", duplicateCommand);

            // Assert
            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
