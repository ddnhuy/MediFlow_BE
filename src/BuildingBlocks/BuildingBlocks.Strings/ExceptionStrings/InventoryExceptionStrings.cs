namespace BuildingBlocks.Strings.ExceptionStrings
{
    public static class InventoryExceptionStrings
    {
        // Warehouse exceptions
        public static string NOT_FOUND_WAREHOUSE_WITH_ID(int id) => $"Không tìm thấy kho với ID \"{id}\".";
        public static string NOT_FOUND_WAREHOUSE_WITH_CODE(string code) => $"Không tìm thấy kho với mã \"{code}\".";
        public static string FAILED_UPDATE_WAREHOUSE_WITH_ID(int id) => $"Cập nhật thông tin kho với ID \"{id}\" thất bại.";
        public static string FAILED_DELETE_WAREHOUSE_WITH_ID(int id) => $"Xóa kho với ID \"{id}\" thất bại.";
        public const string DUPLICATE_WAREHOUSE_CODE = "Mã kho đã tồn tại trong hệ thống.";

        // Warehouse type exceptions
        public static string NOT_FOUND_WAREHOUSE_TYPE_WITH_ID(int id) => $"Không tìm thấy loại kho với ID \"{id}\".";
        public static string NOT_FOUND_WAREHOUSE_TYPE_WITH_CODE(string code) => $"Không tìm thấy loại kho với mã \"{code}\".";

        // Supplier exceptions
        public static string NOT_FOUND_SUPPLIER_WITH_ID(int id) => $"Không tìm thấy nhà cung cấp với ID \"{id}\".";
        public static string NOT_FOUND_SUPPLIER_WITH_CODE(string code) => $"Không tìm thấy nhà cung cấp với mã \"{code}\".";
        public static string FAILED_CREATE_SUPPLIER_WITH_ID => $"Tạo thông tin nhà cung cấp thất bại.";
        public static string FAILED_UPDATE_SUPPLIER_WITH_ID(int id) => $"Cập nhật thông tin nhà cung cấp với ID \"{id}\" thất bại.";
        public static string FAILED_DELETE_SUPPLIER_WITH_ID(int id) => $"Xóa nhà cung cấp với ID \"{id}\" thất bại.";
        public const string DUPLICATE_SUPPLIER_CODE = "Mã nhà cung cấp đã tồn tại trong hệ thống.";

        // Medicine exceptions
        public static string NOT_FOUND_MEDICINE_WITH_ID(int id) => $"Không tìm thấy thuốc với ID \"{id}\".";
        public static string NOT_FOUND_MEDICINE_WITH_CODE(string code) => $"Không tìm thấy thuốc với mã \"{code}\".";
        public static string NOT_FOUND_MEDICINE_WITH_NAME(string name) => $"Không tìm thấy thuốc với tên \"{name}\".";
        public static string FAILED_CREATE_MEDICINE = "Tạo thuốc thất bại.";
        public static string FAILED_UPDATE_MEDICINE_WITH_ID(int id) => $"Cập nhật thông tin thuốc với ID \"{id}\" thất bại.";
        public static string FAILED_DELETE_MEDICINE_WITH_ID(int id) => $"Xóa thuốc với ID \"{id}\" thất bại.";
        public const string DUPLICATE_MEDICINE_CODE = "Mã thuốc đã tồn tại trong hệ thống.";
        public const string MEDICINE_EXPIRED = "Thuốc đã hết hạn sử dụng.";
        public const string MEDICINE_NEAR_EXPIRY = "Thuốc sắp hết hạn sử dụng.";

        // Medicine interaction exceptions
        public static string INCOMPATIBLE_MEDICINES(string medicine1, string medicine2) => $"Thuốc \"{medicine1}\" không tương thích với thuốc \"{medicine2}\".";
        public const string INTERACTION_ALREADY_EXISTS = "Tương tác thuốc đã tồn tại trong hệ thống.";
        public static string FAILED_UPDATE_INTERACTION_WITH_ID(int id) => $"Cập nhật thông tin tương tác thuốc với ID \"{id}\" thất bại.";
        public static string FAILED_DELETE_INTERACTION_WITH_ID(int id) => $"Xóa tương tác thuốc với ID \"{id}\" thất bại.";
        public const string INVALID_INTERACTION_SEVERITY = "Mức độ nghiêm trọng của tương tác thuốc không hợp lệ.";
        public static string NOT_FOUND_INTERACTION_WITH_ID(int id) => $"Không tìm thấy tương tác thuốc với ID \"{id}\".";

        // General inventory exceptions
        public const string INVALID_INVENTORY_OPERATION = "Thao tác kho không hợp lệ.";
        public const string INSUFFICIENT_STOCK = "Số lượng tồn kho không đủ để thực hiện thao tác.";
        public const string DUPLICATE_DOCUMENT = "Mã tài liệu hoặc số tài liệu đã tồn tại trong hệ thống.";
    }
}
