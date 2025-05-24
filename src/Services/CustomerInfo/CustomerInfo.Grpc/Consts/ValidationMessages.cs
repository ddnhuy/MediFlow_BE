namespace CustomerInfo.Grpc.Consts
{
    public static class ValidationMessages
    {
        public const string Code_Required = "Mã bệnh nhân là bắt buộc.";
        public const string Code_MaxLength = "Mã bệnh nhân không được vượt quá {1} ký tự.";

        public const string Name_Required = "Tên bệnh nhân là bắt buộc.";
        public const string Name_MaxLength = "Tên bệnh nhân không được vượt quá {1} ký tự.";

        public const string Gender_Required = "Giới tính là bắt buộc.";
        public const string InvalidGender = "Giới tính không hợp lệ. (0: Nữ, 1: Nam)";

        public const string DOB_Required = "Ngày sinh là bắt buộc.";
        public const string InvalidDate = "Ngày sinh không hợp lệ.";

        public const string Phone_Invalid = "Số điện thoại không hợp lệ.";
        public const string Phone_MaxLength = "Số điện thoại không được vượt quá {1} ký tự.";

        public const string IdentityCard_MaxLength = "CMND/CCCD không được vượt quá {1} ký tự.";

        public const string Address_MaxLength = "Địa chỉ chi tiết không được vượt quá {1} ký tự.";

        public const string Province_MaxLength = "Tỉnh/Thành phố không được vượt quá {1} ký tự.";
        public const string District_MaxLength = "Quận/Huyện không được vượt quá {1} ký tự.";
        public const string Ward_MaxLength = "Xã/Phường không được vượt quá {1} ký tự.";
    }
}