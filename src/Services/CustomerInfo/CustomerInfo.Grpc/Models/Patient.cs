using CustomerInfo.Grpc.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.Grpc.Models
{
    public class Patient : BaseEntity
    {
        [Required(ErrorMessage = "Mã bệnh nhân là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Mã bệnh nhân không được vượt quá {1} ký tự.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên bệnh nhân là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên bệnh nhân không được vượt quá {1} ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới tính là bắt buộc.")]
        [Range(0, 1, ErrorMessage = "Giới tính không hợp lệ. (0: Nữ, 1: Nam)")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không hợp lệ.")]
        public DateTime DOB { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá {1} ký tự.")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "CMND/CCCD không được vượt quá {1} ký tự.")]
        public string? IdentityCard { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ chi tiết không được vượt quá {1} ký tự.")]
        public string? AddressDetail { get; set; }

        [StringLength(100, ErrorMessage = "Tỉnh/Thành phố không được vượt quá {1} ký tự.")]
        public string? Province { get; set; }

        [StringLength(100, ErrorMessage = "Quận/Huyện không được vượt quá {1} ký tự.")]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = "Xã/Phường không được vượt quá {1} ký tự.")]
        public string? Ward { get; set; }

        public bool IsPregnant { get; set; }

        public bool IsForeigner { get; set; }
    }
}
