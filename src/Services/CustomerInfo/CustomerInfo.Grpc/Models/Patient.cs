using CustomerInfo.Grpc.Abstractions;
using CustomerInfo.Grpc.Consts;
using System.ComponentModel.DataAnnotations;

namespace CustomerInfo.Grpc.Models
{
    public class Patient : BaseEntity
    {
        [Required(ErrorMessage = ValidationMessages.Code_Required)]
        [StringLength(50, ErrorMessage = ValidationMessages.Code_MaxLength)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.Name_Required)]
        [StringLength(100, ErrorMessage = ValidationMessages.Name_MaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = ValidationMessages.Gender_Required)]
        [Range(0, 1, ErrorMessage = ValidationMessages.InvalidGender)]
        public int Gender { get; set; }

        [Required(ErrorMessage = ValidationMessages.DOB_Required)]
        [DataType(DataType.Date, ErrorMessage = ValidationMessages.InvalidDate)]
        public DateTime DOB { get; set; }

        [Phone(ErrorMessage = ValidationMessages.Phone_Invalid)]
        [StringLength(20, ErrorMessage = ValidationMessages.Phone_MaxLength)]
        public string? PhoneNumber { get; set; }

        [StringLength(50, ErrorMessage = ValidationMessages.IdentityCard_MaxLength)]
        public string? IdentityCard { get; set; }

        [StringLength(200, ErrorMessage = ValidationMessages.Address_MaxLength)]
        public string? AddressDetail { get; set; }

        [StringLength(100, ErrorMessage = ValidationMessages.Province_MaxLength)]
        public string? Province { get; set; }

        [StringLength(100, ErrorMessage = ValidationMessages.District_MaxLength)]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = ValidationMessages.Ward_MaxLength)]
        public string? Ward { get; set; }

        public bool IsPregnant { get; set; }

        public bool IsForeigner { get; set; }
    }
}