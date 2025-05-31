namespace BuildingBlocks.Strings
{
    public static class ValidationStrings
    {
        public const string REQUIRED_USERNAME = "Tên người dùng là bắt buộc.";
        public const string REQUIRED_PASSWORD = "Mật khẩu là bắt buộc.";
        public const string INVALID_PASSWORD_LENGTH = "Mật khẩu phải dài ít nhất 8 kí tự, bao gồm ít nhất 1 kí tự in hoa, 1 kí tự thường, 1 chữ số và 1 kí tự đặc biệt.";
        public const string INVALID_NEW_PASSWORD = "Mật khẩu mới không được trùng với mật khẩu hiện tại.";

        public const string REQUIRED_REFRESH_TOKEN = "Refresh Token là bắt buộc.";

        public const string REQUIRED_USER_ID = "Mã người dùng là bắt buộc.";
        public const string REQUIRED_CURRENT_PASSWORD = "Mật khẩu cũ là bắt buộc.";
        public const string REQUIRED_NEW_PASSWORD = "Mật khẩu mới là bắt buộc.";

        public const string REQUIRED_EMAIL = "Email là bắt buộc.";
        public const string INVALID_EMAIL = "Email không hợp lệ.";

        // Warehouse validation
        public const string REQUIRED_WAREHOUSE_CODE = "Mã kho là bắt buộc.";
        public const string REQUIRED_WAREHOUSE_NAME = "Tên kho là bắt buộc.";
        public const string INVALID_WAREHOUSE_CODE_FORMAT = "Mã kho không đúng định dạng.";
        public const string REQUIRED_WAREHOUSE_TYPE = "Loại kho là bắt buộc.";
        public const string INVALID_WAREHOUSE_TYPE = "Loại kho không hợp lệ.";

        // Supplier validation
        public const string REQUIRED_SUPPLIER_CODE = "Mã nhà cung cấp là bắt buộc.";
        public const string REQUIRED_SUPPLIER_NAME = "Tên nhà cung cấp là bắt buộc.";
        public const string REQUIRED_SUPPLIER_ADDRESS = "Địa chỉ nhà cung cấp là bắt buộc.";
        public const string REQUIRED_SUPPLIER_CONTACT_PERSON = "Thông tin người liên hệ là bắt buộc.";
        public const string REQUIRED_SUPPLIER_DIRECTOR = "Thông tin giám đốc là bắt buộc.";
        public const string REQUIRED_SUPPLIER_FAX = "Số Fax là bắt buộc.";
        public const string INVALID_SUPPLIER_CODE_FORMAT = "Mã nhà cung cấp không đúng định dạng.";
        public const string INVALID_SUPPLIER_PHONE = "Số điện thoại nhà cung cấp không hợp lệ.";
        public const string INVALID_SUPPLIER_EMAIL = "Email nhà cung cấp không hợp lệ.";
        public const string INVALID_SUPPLIER_TAX_CODE = "Mã số thuế không hợp lệ.";

        // Medicine validation
        public const string REQUIRED_MEDICINE_CODE = "Mã thuốc là bắt buộc.";
        public const string REQUIRED_MEDICINE_NAME = "Tên thuốc là bắt buộc.";
        public const string INVALID_MEDICINE_CODE_FORMAT = "Mã thuốc không đúng định dạng.";
        public const string REQUIRED_MEDICINE_UNIT = "Đơn vị tính là bắt buộc.";
        public const string REQUIRED_MEDICINE_CATEGORY = "Nhóm thuốc là bắt buộc.";
        public const string INVALID_MEDICINE_PRICE = "Giá thuốc không hợp lệ.";
        public const string INVALID_MEDICINE_QUANTITY = "Số lượng thuốc không hợp lệ.";
        public const string INVALID_MEDICINE_EXPIRY_DATE = "Ngày hết hạn không hợp lệ.";
        public const string INVALID_MEDICINE_MANUFACTURE_DATE = "Ngày sản xuất không hợp lệ.";
        public const string INVALID_DATE_RANGE = "Ngày sản xuất phải trước ngày hết hạn.";
        public const string REQUIRED_MEDICINE_ROUTE = "Đường dùng thuốc là bắt buộc.";
        public const string REQUIRED_MEDICINE_DOSAGE_FORM = "Dạng bào chế là bắt buộc.";
        public const string REQUIRED_MANUFACTURER = "Nhà sản xuất là bắt buộc.";
        public const string REQUIRED_ACTIVE_INGREDIENT = "Hoạt chất là bắt buộc.";
        public const string REQUIRED_USAGE_INSTRUCTIONS = "Hướng dẫn sử dụng là bắt buộc.";
        public const string REQUIRED_CONCENTRATION = "Nồng độ/Hàm lượng là bắt buộc.";
        public const string REQUIRED_INDICATIONS = "Chỉ định là bắt buộc.";
        public const string REQUIRED_MEDICINE_CLASSIFICATION = "Phân loại thuốc là bắt buộc.";
        public const string REQUIRED_ROUTE_OF_ADMINISTRATION = "Đường dùng là bắt buộc.";
        public const string REQUIRED_NATIONAL_MEDICINE_CODE = "Mã thuốc quốc gia là bắt buộc.";
        public const string REQUIRED_REGISTRATION_NUMBER = "Số đăng ký là bắt buộc.";
        public const string REQUIRED_VALID_MEDICINE_TYPE = "Loại thuốc hợp lệ phải được chọn.";
        public const string REQUIRED_VALID_VACCINE_TYPE = "Loại vaccine hợp lệ phải được chọn.";

        // Medicine interaction validation
        public const string REQUIRED_FIRST_MEDICINE = "Thuốc thứ nhất là bắt buộc.";
        public const string REQUIRED_SECOND_MEDICINE = "Thuốc thứ hai là bắt buộc.";
        public const string SAME_MEDICINE_INTERACTION = "Không thể tạo tương tác giữa cùng một loại thuốc.";
        public const string REQUIRED_INTERACTION_SEVERITY = "Mức độ nghiêm trọng là bắt buộc.";
        public const string REQUIRED_INTERACTION_EFFECT = "Tác dụng tương tác là bắt buộc.";
        public const string INVALID_SEVERITY_LEVEL = "Mức độ nghiêm trọng không hợp lệ.";

        // Inventory operation validation
        public const string REQUIRED_QUANTITY = "Số lượng là bắt buộc.";
        public const string INVALID_QUANTITY = "Số lượng không hợp lệ.";
        public const string REQUIRED_OPERATION_DATE = "Ngày thực hiện là bắt buộc.";
        public const string INVALID_OPERATION_DATE = "Ngày thực hiện không hợp lệ.";
        public const string REQUIRED_OPERATION_TYPE = "Loại thao tác là bắt buộc.";
        public const string REQUIRED_SOURCE_WAREHOUSE = "Kho nguồn là bắt buộc.";
        public const string REQUIRED_DESTINATION_WAREHOUSE = "Kho đích là bắt buộc.";
        public const string SAME_WAREHOUSE_TRANSFER = "Không thể chuyển kho trong cùng một kho.";

        // Import medicine validation
        public const string REQUIRED_WAREHOUSE_ID = "Mã kho là bắt buộc.";
        public const string REQUIRED_SUPPLIER_ID = "Mã nhà cung cấp là bắt buộc.";
        public const string REQUIRED_RECEIVER_ID = "Mã người nhận là bắt buộc.";
        public const string REQUIRED_DOCUMENT_CODE = "Mã tài liệu là bắt buộc.";
        public const string REQUIRED_DOCUMENT_NUMBER = "Số tài liệu là bắt buộc.";
        public const string REQUIRED_IMPORT_DATE = "Ngày nhập là bắt buộc.";
        public const string REQUIRED_MEDICINE_DETAIL = "Cần ít nhất một chi tiết thuốc.";
        public const string REQUIRED_MEDICINE_ID = "ID thuốc là bắt buộc.";
        public const string REQUIRED_BATCH_NUMBER = "Số lô là bắt buộc.";
        public const string QUANTITY_GREATER_THAN_ZERO = "Số lượng phải lớn hơn không.";
        public const string UNIT_PRICE_NON_NEGATIVE = "Đơn giá phải lớn hơn hoặc bằng không.";
        public const string REQUIRED_EXPIRY_DATE = "Ngày hết hạn là bắt buộc.";
        public const string EXPIRY_DATE_FUTURE = "Ngày hết hạn phải trong tương lai.";
        public const string REQUIRED_MANUFACTURER_ID = "ID nhà sản xuất là bắt buộc.";
        public const string REQUIRED_COUNTRY_ID = "ID quốc gia là bắt buộc.";

        // String length validation
        public static string MAX_LENGTH(string fieldName, int maxLength) => $"{fieldName} không được vượt quá {maxLength} kí tự.";
        public static string MIN_LENGTH(string fieldName, int minLength) => $"{fieldName} phải có ít nhất {minLength} kí tự.";
        public static string EXACT_LENGTH(string fieldName, int length) => $"{fieldName} phải có đúng {length} kí tự.";
        public static string LENGTH_RANGE(string fieldName, int minLength, int maxLength) => $"{fieldName} phải có từ {minLength} đến {maxLength} kí tự.";
    }
}
