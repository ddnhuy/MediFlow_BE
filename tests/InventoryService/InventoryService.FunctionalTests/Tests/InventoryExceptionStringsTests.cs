using BuildingBlocks.Strings.ExceptionStrings;
using FluentAssertions;
using Xunit;

namespace Inventory.FunctionalTests.Tests
{
    public class InventoryExceptionStringsTests
    {
        [Fact]
        public void WarehouseExceptions_ReturnExpectedMessages()
        {
            // Arrange
            int testId = 123;
            string testCode = "WH001";

            // Act & Assert
            InventoryExceptionStrings.NOT_FOUND_WAREHOUSE_WITH_ID(testId)
                .Should().Be($"Không tìm thấy kho với ID \"{testId}\".");

            InventoryExceptionStrings.NOT_FOUND_WAREHOUSE_WITH_CODE(testCode)
                .Should().Be($"Không tìm thấy kho với mã \"{testCode}\".");

            InventoryExceptionStrings.FAILED_UPDATE_WAREHOUSE_WITH_ID(testId)
                .Should().Be($"Cập nhật thông tin kho với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.FAILED_DELETE_WAREHOUSE_WITH_ID(testId)
                .Should().Be($"Xóa kho với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.DUPLICATE_WAREHOUSE_CODE
                .Should().Be("Mã kho đã tồn tại trong hệ thống.");
        }

        [Fact]
        public void WarehouseTypeExceptions_ReturnExpectedMessages()
        {
            // Arrange
            int testId = 456;
            string testCode = "WHT001";

            // Act & Assert
            InventoryExceptionStrings.NOT_FOUND_WAREHOUSE_TYPE_WITH_ID(testId)
                .Should().Be($"Không tìm thấy loại kho với ID \"{testId}\".");

            InventoryExceptionStrings.NOT_FOUND_WAREHOUSE_TYPE_WITH_CODE(testCode)
                .Should().Be($"Không tìm thấy loại kho với mã \"{testCode}\".");
        }

        [Fact]
        public void SupplierExceptions_ReturnExpectedMessages()
        {
            // Arrange
            int testId = 789;
            string testCode = "SUP001";

            // Act & Assert
            InventoryExceptionStrings.NOT_FOUND_SUPPLIER_WITH_ID(testId)
                .Should().Be($"Không tìm thấy nhà cung cấp với ID \"{testId}\".");

            InventoryExceptionStrings.NOT_FOUND_SUPPLIER_WITH_CODE(testCode)
                .Should().Be($"Không tìm thấy nhà cung cấp với mã \"{testCode}\".");

            InventoryExceptionStrings.FAILED_UPDATE_SUPPLIER_WITH_ID(testId)
                .Should().Be($"Cập nhật thông tin nhà cung cấp với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.FAILED_DELETE_SUPPLIER_WITH_ID(testId)
                .Should().Be($"Xóa nhà cung cấp với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.DUPLICATE_SUPPLIER_CODE
                .Should().Be("Mã nhà cung cấp đã tồn tại trong hệ thống.");
        }

        [Fact]
        public void MedicineExceptions_ReturnExpectedMessages()
        {
            // Arrange
            int testId = 101;
            string testCode = "MED001";
            string testName = "Test Medicine";

            // Act & Assert
            InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_ID(testId)
                .Should().Be($"Không tìm thấy thuốc với ID \"{testId}\".");

            InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_CODE(testCode)
                .Should().Be($"Không tìm thấy thuốc với mã \"{testCode}\".");

            InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_NAME(testName)
                .Should().Be($"Không tìm thấy thuốc với tên \"{testName}\".");

            InventoryExceptionStrings.FAILED_CREATE_MEDICINE
                .Should().Be("Tạo thuốc thất bại.");

            InventoryExceptionStrings.FAILED_UPDATE_MEDICINE_WITH_ID(testId)
                .Should().Be($"Cập nhật thông tin thuốc với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.FAILED_DELETE_MEDICINE_WITH_ID(testId)
                .Should().Be($"Xóa thuốc với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.DUPLICATE_MEDICINE_CODE
                .Should().Be("Mã thuốc đã tồn tại trong hệ thống.");

            InventoryExceptionStrings.MEDICINE_EXPIRED
                .Should().Be("Thuốc đã hết hạn sử dụng.");

            InventoryExceptionStrings.MEDICINE_NEAR_EXPIRY
                .Should().Be("Thuốc sắp hết hạn sử dụng.");
        }

        [Fact]
        public void MedicineInteractionExceptions_ReturnExpectedMessages()
        {
            // Arrange
            string medicine1 = "Aspirin";
            string medicine2 = "Warfarin";
            int testId = 505;

            // Act & Assert
            InventoryExceptionStrings.INCOMPATIBLE_MEDICINES(medicine1, medicine2)
                .Should().Be($"Thuốc \"{medicine1}\" không tương thích với thuốc \"{medicine2}\".");

            InventoryExceptionStrings.INTERACTION_ALREADY_EXISTS
                .Should().Be("Tương tác thuốc đã tồn tại trong hệ thống.");

            InventoryExceptionStrings.FAILED_UPDATE_INTERACTION_WITH_ID(testId)
                .Should().Be($"Cập nhật thông tin tương tác thuốc với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.FAILED_DELETE_INTERACTION_WITH_ID(testId)
                .Should().Be($"Xóa tương tác thuốc với ID \"{testId}\" thất bại.");

            InventoryExceptionStrings.INVALID_INTERACTION_SEVERITY
                .Should().Be("Mức độ nghiêm trọng của tương tác thuốc không hợp lệ.");

            InventoryExceptionStrings.NOT_FOUND_INTERACTION_WITH_ID(testId)
                .Should().Be($"Không tìm thấy tương tác thuốc với ID \"{testId}\".");
        }

        [Fact]
        public void GeneralInventoryExceptions_ReturnExpectedMessages()
        {
            // Act & Assert
            InventoryExceptionStrings.INVALID_INVENTORY_OPERATION
                .Should().Be("Thao tác kho không hợp lệ.");

            InventoryExceptionStrings.INSUFFICIENT_STOCK
                .Should().Be("Số lượng tồn kho không đủ để thực hiện thao tác.");
        }
    }
}
