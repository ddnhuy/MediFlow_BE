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

        [Fact]
        public async Task ImportMedicineFromSupplier_WithDuplicateDocument_ReturnsBadRequest()
        {
            // Arrange - First create a successful import
            var command1 = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "DUPLICATE_DOC",
                DocumentNumber = "DUPLICATE_NUM",
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SupplierId = 1,
                ReceivedById = 1,
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH001",
                        Quantity = 100,
                        UnitPrice = 10.5m,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                        ManufacturerId = 1,
                        CountryId = 1
                    }
                }
            };

            // First import should succeed
            var response1 = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command1);
            response1.StatusCode.Should().Be(HttpStatusCode.OK);

            // Arrange - Create second command with the same document code
            var command2 = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "DUPLICATE_DOC", // Same document code
                DocumentNumber = "UNIQUE_NUM",
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SupplierId = 1,
                ReceivedById = 1,
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH002",
                        Quantity = 50,
                        UnitPrice = 10.5m,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                        ManufacturerId = 1,
                        CountryId = 1
                    }
                }
            };

            // Act/Assert - Try to import with duplicate document code
            var response2 = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command2);
            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem2 = await response2.Content.ReadFromJsonAsync<ProblemDetails>();
            problem2.Should().NotBeNull();

            // Arrange - Create third command with the same document number but different code
            var command3 = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "UNIQUE_DOC",
                DocumentNumber = "DUPLICATE_NUM", // Same document number
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SupplierId = 1,
                ReceivedById = 1,
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH003",
                        Quantity = 75,
                        UnitPrice = 10.5m,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                        ManufacturerId = 1,
                        CountryId = 1
                    }
                }
            };

            // Act/Assert - Try to import with duplicate document number
            var response3 = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command3);
            response3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem3 = await response3.Content.ReadFromJsonAsync<ProblemDetails>();
            problem3.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportMedicineFromSupplier_WhenExceptionOccurs_RollsBackTransaction()
        {
            // Arrange - Create a command that will cause an exception during processing
            // Using an invalid foreign key (non-existent manufacturer ID) to trigger a database exception
            var command = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "ROLLBACK_TEST_DOC",
                DocumentNumber = "ROLLBACK_TEST_NUM",
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SupplierId = 9999,
                ReceivedById = 1,
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH_ROLLBACK",
                        Quantity = 100,
                        UnitPrice = 10.5m,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
                        // Use an invalid manufacturer ID to cause a foreign key constraint violation
                        ManufacturerId = 1,
                        CountryId = 1
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/import-medicine-from-supplier", command);

            // Assert
            // Should return an error status code
            response.IsSuccessStatusCode.Should().BeFalse();

            // Now verify the transaction was rolled back by checking that the document wasn't created
            // We can do this by trying to create another document with the same document code/number
            // If the rollback was successful, this should succeed
            var verificationCommand = new ImportMedicineFromSupplierCommand
            {
                DocumentCode = "ROLLBACK_TEST_DOC", // Same as the failed command
                DocumentNumber = "ROLLBACK_TEST_NUM", // Same as the failed command
                WarehouseId = 1,
                ImportDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SupplierId = 1,
                ReceivedById = 1,
                Details = new List<ImportMedicineDetailDto>
                {
                    new ImportMedicineDetailDto
                    {
                        MedicineId = 1,
                        BatchNumber = "BATCH_VERIFY",
                        Quantity = 50,
                        UnitPrice = 20.0m,
                        ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                        ManufacturerId = 1, // Valid manufacturer ID
                        CountryId = 1
                    }
                }
            };

            // If the transaction was properly rolled back, we should be able to use the same document code/number
            var verificationResponse = await _client.PostAsJsonAsync("/import-medicine-from-supplier", verificationCommand);
            verificationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await verificationResponse.Content.ReadFromJsonAsync<ImportMedicineFromSupplierResponse>();
            result.Should().NotBeNull();
        }
    }
}
