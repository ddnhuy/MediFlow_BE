using FluentValidation;
using VaccinationReception.Application.DTOs.VaccinationReceptionDTOs;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.Application.VaccinationReceptions.Validators
{
    public class CreatePatientReceptionCommandValidator : AbstractValidator<CreatePatientReceptionCommand>
    {
        public CreatePatientReceptionCommandValidator()
        {
            RuleFor(x => x.createPatientCommand)
                .NotNull()
                .WithMessage("Thông tin bệnh nhân không được để trống");

            RuleFor(x => x.createReceptionDTO)
                .NotNull()
                .WithMessage("Thông tin tiếp đón không được để trống");

            RuleFor(x => x.createPatientCommand.Code)
                .NotEmpty()
                .WithMessage("Mã bệnh nhân không được để trống")
                .MaximumLength(20)
                .WithMessage("Mã bệnh nhân không được vượt quá 20 ký tự");

            RuleFor(x => x.createPatientCommand.Name)
                .NotEmpty()
                .WithMessage("Tên bệnh nhân không được để trống")
                .MaximumLength(100)
                .WithMessage("Tên bệnh nhân không được vượt quá 100 ký tự");

            RuleFor(x => x.createPatientCommand.Gender)
                .IsInEnum()
                .WithMessage("Giới tính không hợp lệ");

            RuleFor(x => x.createPatientCommand.Dob)
                .NotEmpty()
                .WithMessage("Ngày sinh không được để trống")
                .LessThan(DateTime.Now)
                .WithMessage("Ngày sinh không được lớn hơn ngày hiện tại");

            RuleFor(x => x.createPatientCommand.PhoneNumber)
                .NotEmpty()
                .WithMessage("Số điện thoại không được để trống")
                .Matches(@"^[0-9]{10,11}$")
                .WithMessage("Số điện thoại không hợp lệ (10-11 số)");

            RuleFor(x => x.createPatientCommand.IdentityCard)
                .NotEmpty()
                .WithMessage("Số CMND/CCCD không được để trống")
                .Matches(@"^[0-9]{9,12}$")
                .WithMessage("Số CMND/CCCD không hợp lệ (9-12 số)");

            RuleFor(x => x.createPatientCommand.AddressDetail)
                .NotEmpty()
                .WithMessage("Địa chỉ chi tiết không được để trống")
                .MaximumLength(200)
                .WithMessage("Địa chỉ chi tiết không được vượt quá 200 ký tự");

            RuleFor(x => x.createPatientCommand.Province)
                .NotEmpty()
                .WithMessage("Tỉnh/Thành phố không được để trống")
                .MaximumLength(100)
                .WithMessage("Tỉnh/Thành phố không được vượt quá 100 ký tự");

            RuleFor(x => x.createPatientCommand.District)
                .NotEmpty()
                .WithMessage("Quận/Huyện không được để trống")
                .MaximumLength(100)
                .WithMessage("Quận/Huyện không được vượt quá 100 ký tự");

            RuleFor(x => x.createPatientCommand.Ward)
                .NotEmpty()
                .WithMessage("Phường/Xã không được để trống")
                .MaximumLength(100)
                .WithMessage("Phường/Xã không được vượt quá 100 ký tự");

            // Validate CreateReceptionDTO
            RuleFor(x => x.createReceptionDTO.PatientId)
                .GreaterThan(0)
                .When(x => x.patientId > 0)
                .WithMessage("Mã bệnh nhân không hợp lệ");

            RuleFor(x => x.createReceptionDTO.ReceptionDate)
                .NotEmpty()
                .WithMessage("Ngày tiếp đón không được để trống")
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Ngày tiếp đón không được nhỏ hơn ngày hiện tại");

            RuleFor(x => x.createReceptionDTO.ServiceTypeId)
                .GreaterThan(0)
                .WithMessage("Loại dịch vụ không hợp lệ");

        }
    }
}