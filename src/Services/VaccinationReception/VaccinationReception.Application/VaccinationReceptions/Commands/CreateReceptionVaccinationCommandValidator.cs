using BuildingBlocks.Strings;
using FluentValidation;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.Application.VaccinationReceptions.Validators
{
    public class CreateReceptionVaccinationCommandValidator : AbstractValidator<CreateReceptionVaccinationCommand>
    {
        public CreateReceptionVaccinationCommandValidator()
        {
            RuleFor(x => x.ReceptionId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID.ToString());

            RuleFor(x => x.VaccineId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_VACCINE_ID.ToString());

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_QUANTITY.ToString());

            RuleFor(x => x.ScheduledDate)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_SCHEDULED_DATE.ToString())
                .Must(date =>date >= DateTime.Now)
                .WithMessage(ExceptionKey.INVALID_SCHEDULED_DATE.ToString());

            RuleFor(x => x.AppointmentDate)
                .Must(date => date == null || date >= DateTime.Now)
                .WithMessage(ExceptionKey.INVALID_APPOINTMENT_DATE.ToString());

            // Validate Note if provided
            When(x => !string.IsNullOrEmpty(x.Note), () =>
            {
                RuleFor(x => x.Note)
                    .MaximumLength(500)
                    .WithMessage(ExceptionKey.INVALID_NOTE_MAX_LENGTH.ToString());
            });

            RuleFor(x => x)
                .Must(command =>
                {
                    if (command.IsReadyToUse && command.ScheduledDate == default)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(ExceptionKey.INVALID_DATA_FOLLOWING_BUSINESS_RULES.ToString());
        }
    }
}