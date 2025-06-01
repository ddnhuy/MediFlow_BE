using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class AddServiceToRequestFormCommandValidator : AbstractValidator<AddServiceToRequestFormCommand>
    {
        public AddServiceToRequestFormCommandValidator()
        {
            RuleFor(x => x.ReceptionId)
                .GreaterThan(0)
                .WithMessage("Mã tiếp đón không hợp lệ");

            RuleFor(x => x.Services)
                .Must((command, services) =>
                {
                    if (services == null || !services.Any())
                    {
                        return !string.IsNullOrEmpty(command.GroupType) && command.GroupId.HasValue;
                    }
                    return true;
                })
                .WithMessage("Phải cung cấp danh sách dịch vụ hoặc nhóm dịch vụ");

            RuleFor(x => x.GroupType)
                .Must((command, groupType) =>
                {
                    if (!string.IsNullOrEmpty(groupType))
                    {
                        return command.GroupId.HasValue;
                    }
                    return true;
                })
                .WithMessage("Phải cung cấp mã nhóm khi chọn loại nhóm");

            RuleFor(x => x.GroupId)
                .Must((command, groupId) =>
                {
                    if (groupId.HasValue)
                    {
                        return !string.IsNullOrEmpty(command.GroupType);
                    }
                    return true;
                })
                .WithMessage("Phải cung cấp loại nhóm khi chọn mã nhóm");

            RuleFor(x => x.Services)
                .Must((command, services) =>
                {
                    if (services == null || !services.Any())
                    {
                        return true;
                    }

                    return services.All(s => s.ServiceId > 0 && s.Quantity > 0);
                })
                .WithMessage("Danh sách dịch vụ không hợp lệ");

            RuleForEach(x => x.Services)
                .ChildRules(service =>
                {
                    service.RuleFor(x => x.ServiceId)
                        .GreaterThan(0)
                        .WithMessage("Mã dịch vụ không hợp lệ");

                    service.RuleFor(x => x.Quantity)
                        .GreaterThan(0)
                        .WithMessage("Số lượng phải lớn hơn 0");
                });

            RuleFor(x => x.DefaultQuantity)
                .GreaterThan(0)
                .WithMessage("Số lượng mặc định phải lớn hơn 0");
        }
    }
}