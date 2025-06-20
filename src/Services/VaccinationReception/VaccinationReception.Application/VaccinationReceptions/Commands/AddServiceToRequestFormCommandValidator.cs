using BuildingBlocks.Strings;
using FluentValidation;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class AddServiceToRequestFormCommandValidator : AbstractValidator<AddServiceToRequestFormCommand>
    {
        public AddServiceToRequestFormCommandValidator()
        {
            RuleFor(x => x.ReceptionId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID.ToString());

            RuleFor(x => x.Services)
                .Must((command, services) =>
                {
                    if (services == null || !services.Any())
                    {
                        return !string.IsNullOrEmpty(command.GroupType) && command.GroupId.HasValue;
                    }
                    return true;
                })
                .WithMessage(ExceptionKey.INVALID_SERVICE_LIST.ToString());

            RuleFor(x => x.GroupType)
                .Must((command, groupType) =>
                {
                    if (!string.IsNullOrEmpty(groupType))
                    {
                        return command.GroupId.HasValue;
                    }
                    return true;
                })
                .WithMessage(ExceptionKey.INVALID_GROUP_TYPE.ToString());

            RuleFor(x => x.GroupId)
                .Must((command, groupId) =>
                {
                    if (groupId.HasValue)
                    {
                        return !string.IsNullOrEmpty(command.GroupType);
                    }
                    return true;
                })
                .WithMessage(ExceptionKey.INVALID_GROUP_ID.ToString());

            RuleFor(x => x.Services)
                .Must((command, services) =>
                {
                    if (services == null || !services.Any())
                    {
                        return true;
                    }

                    return services.All(s => s.ServiceId > 0 && s.Quantity > 0);
                })
                .WithMessage(ExceptionKey.INVALID_SERVICE_LIST.ToString());

            RuleForEach(x => x.Services)
                .ChildRules(service =>
                {
                    service.RuleFor(x => x.ServiceId)
                        .GreaterThan(0)
                        .WithMessage(ExceptionKey.INVALID_SERVICE_ID.ToString());

                    service.RuleFor(x => x.Quantity)
                        .GreaterThan(0)
                        .WithMessage(ExceptionKey.INVALID_QUANTITY.ToString());
                });

            RuleFor(x => x.DefaultQuantity)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_DEFAULT_QUANTITY.ToString());
        }
    }
}