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
    }
}
