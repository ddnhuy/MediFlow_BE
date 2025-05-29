namespace BuildingBlocks.Strings.Exceptions
{
    public static class HumanResourceExceptionStrings
    {
        public static string NOT_FOUND_USER_WITH_ID(int id) => $"Không tìm thấy người dùng với ID \"{id}\".";
        public static string NOT_FOUND_USER_WITH_EMAIL(string email) => $"Không tìm thấy người dùng với email \"{email}\".";
        public const string INVALID_LOGIN_CREDENTIAL = "Tên người dùng hoặc mật khẩu không chính xác, vui lòng thử lại.";

        public static string FAILED_UPDATE_USER_WITH_ID(int id) => $"Cập nhật thông tin người dùng với ID \"{id}\" thất bại.";
        public const string FAILED_RESET_PASSWORD = "Đặt lại mật khẩu không thành công. Vui lòng thử lại.";
        public const string FAILED_CHANGE_PASSWORD = "Mật khẩu hiện tại chưa chính xác hoặc mật khẩu mới không hợp lệ, vui lòng thử lại.";
        public const string FAILED_ASSIGN_ROLE_TO_USER = "Gán vai trò cho người dùng không thành công, vui lòng thử lại.";

        public static string NOT_FOUND_ROLE_WITH_ID(int id) => $"Không tìm thấy vai trò với ID \"{id}\".";
        public static string NOT_FOUND_DEPARTMENT_WITH_ID(int id) => $"Không tìm thấy phòng ban với ID \"{id}\".";
        public static string NOT_FOUND_PERMISSION_WITH_ID(int id) => $"Không tìm thấy quyền truy cập với ID \"{id}\".";

        public static string NOT_FOUND_POLICY_WITH_ID(int id) => $"Không tìm thấy chính sách với ID \"{id}\".";
        public static string CANNOT_DELETE_POLICY_WITH_RELATIONSHIPS(int id) => $"Không thể xoá chính sách với ID \"{id}\", vui lòng thử lại.";

        public static string POLICY_ASSIGNMENT_ALREADY_EXISTS => $"Chính sách đã được gán cho vai trò và phòng ban mà bạn đã chọn.";
        public const string EXISTED_DEPARTMENT_CODE = "Mã phòng ban đã tồn tại.";

        public const string INVALID_REQUEST = "Yêu cầu không hợp lệ.";
        public const string INVALID_DEPARTMENT_TYPE = "Loại phòng ban không hợp lệ.";
    }
}
