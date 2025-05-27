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

    }
}
