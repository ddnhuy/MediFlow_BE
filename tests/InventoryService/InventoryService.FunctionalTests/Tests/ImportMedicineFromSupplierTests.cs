using FluentAssertions;
using Inventory.API.Endpoints;
using Inventory.Application.Medicines.Commands.ImportMedicineFromSupplier;
using InventoryService.FunctionalTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class ImportMedicineFromSupplierTests : BaseFunctionalTest
    {
        public ImportMedicineFromSupplierTests(FunctionalTestWebAppFactory factory) : base(factory) { }

        [Fact]
        public async Task ImportMedicineFromSupplier_WithValidData_ReturnsOk()
        {
            // Arrange
            var command = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "DOC001",
                DocumentNumber = "IMP20240501",
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SupplierId = 1,
                Note = "Test import",
                ReceivedById = 1,
                SupportingDocument = "invoice.pdf",
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH001",
                        SGK_CPNK = "SGK123",
                        Note = "First batch",
                        Quantity = 100,
                        UnitPrice = 10.5m,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                        ManufacturerId = 1,
                        CountryId = 1,
                        IsFree = false
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<ImportMedicineFromSupplierResponse>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportMedicineFromSupplier_WhenUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            var command = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "DOC001",
                SupplierId = 1,
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH001",
                        Quantity = 100,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                        ManufacturerId = 1,
                        CountryId = 1
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ImportMedicineFromSupplier_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new ImportMedicineFromSupplierCommand
            {
                // Missing required fields
                SupplierId = 0,
                WarehouseId = 0,
                // Empty details collection
                Details = new List<ImportMedicineDetailDto>()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
        }
    }
}
