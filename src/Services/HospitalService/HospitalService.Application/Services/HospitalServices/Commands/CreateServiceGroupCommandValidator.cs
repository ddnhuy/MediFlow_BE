using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateServiceGroupCommandValidator : AbstractValidator<CreateServiceGroupCommand>
    {
        public CreateServiceGroupCommandValidator()
        {

            RuleFor(x => x.GroupName)
                .NotEmpty()
                .WithMessage("Tên nhóm dịch vụ không được để trống")
                .MaximumLength(200)
                .WithMessage("Tên nhóm dịch vụ không được vượt quá 200 ký tự")
                .MinimumLength(3)
                .WithMessage("Tên nhóm dịch vụ phải có ít nhất 3 ký tự");
  
            RuleForEach(x => x.ServiceIds)
                .GreaterThan(0)
                .WithMessage("Mã dịch vụ không hợp lệ")
                .When(x => x.ServiceIds != null && x.ServiceIds.Any());
        }
    }
}
