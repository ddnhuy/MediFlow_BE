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
                SupplierName: "Test Supplier",
                Address: "123 Test Street",
                Phone: "0981995925",
                Fax: "+1-212-555-0102",
                Email: "contact@testsupplier.com",
                TaxCode: "TAX12345",
                Director: "John Director",
                ContactPerson: "Jane Contact",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: null
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
                SupplierName: "", // Empty supplier name
                Address: "123 Test Street",
                Phone: "0981995925",
                Fax: "+1-212-555-0102",
                Email: "contact@testsupplier.com",
                TaxCode: "TAX12345",
                Director: "John Director",
                ContactPerson: "Jane Contact",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>()
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "contract1.pdf"),
                    new CreateSupplierContractRequest(Guid.NewGuid(), "contract2.pdf")
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
        public async Task Create_WithValidData_ReturnsCreated()
        {
            var command = new CreateSupplierCommand(      
                SupplierName: "Valid Supplier",
                Address: "123 Valid St",
                Phone: "1234567890",
                Fax: "1234567891",
                Email: "valid@supplier.com",
                TaxCode: "TAX100",
                Director: "Director Name",
                ContactPerson: "Contact Name",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: new List<CreateSupplierContractRequest>()
                {
                    new CreateSupplierContractRequest(Guid.NewGuid(), "contract1.pdf"),
                    new CreateSupplierContractRequest(Guid.NewGuid(), "contract2.pdf")
                }
            );

            var response = await _client.PostAsJsonAsync("/suppliers", command);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_WhenUnauthorized_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var command = new CreateSupplierCommand(
                SupplierName: "Unauthorized Supplier",
                Address: "123 Unauthorized St",
                Phone: "1234567890",
                Fax: "1234567891",
                Email: "unauth@supplier.com",
                TaxCode: "TAX101",
                Director: "Director Name",
                ContactPerson: "Contact Name",
                ExpiredDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                Contracts: null
            );

            var response = await _client.PostAsJsonAsync("/suppliers", command);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
