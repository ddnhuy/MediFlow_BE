using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
    {
        public CreateServiceCommandValidator()
        {
            RuleFor(x => x.ServiceCode)
                .NotEmpty()
                .WithMessage("Mã dịch vụ không được để trống")
                .MaximumLength(50)
                .WithMessage("Mã dịch vụ không được vượt quá 50 ký tự")
                .Matches("^[A-Za-z0-9-_]+$")
                .WithMessage("Mã dịch vụ chỉ được chứa chữ cái, số, dấu gạch ngang và dấu gạch dưới");

            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .WithMessage("Tên dịch vụ không được để trống")
                .MaximumLength(200)
                .WithMessage("Tên dịch vụ không được vượt quá 200 ký tự")
                .MinimumLength(3)
                .WithMessage("Tên dịch vụ phải có ít nhất 3 ký tự");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Đơn giá phải lớn hơn 0")
                .LessThanOrEqualTo(1000000000)
                .WithMessage("Đơn giá không được vượt quá 1 tỷ");
        }
    }
}
