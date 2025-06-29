using BuildingBlocks.Strings;
using FluentValidation;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.Application.VaccinationReceptions.Validators
{
    public class CreateScreeningEvaluationReportCommandValidator : AbstractValidator<CreateScreeningEvaluationReportCommand>
    {
        public CreateScreeningEvaluationReportCommandValidator()
        {
            RuleFor(x => x.ParentFullName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.ParentFullName))
                .WithMessage(ExceptionKey.INVALID_PARENT_FULL_NAME_MAX_LENGTH.ToString());

            RuleFor(x => x.ParentPhoneNumber)
                .Matches(@"^[0-9]{10,11}$")
                .When(x => !string.IsNullOrEmpty(x.ParentPhoneNumber))
                .WithMessage(ExceptionKey.INVALID_PARENT_PHONE_FORMAT.ToString());

            RuleFor(x => x.WeightKg)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_WEIGHT.ToString())
                .LessThanOrEqualTo(200)
                .WithMessage(ExceptionKey.INVALID_WEIGHT.ToString());

            RuleFor(x => x.BodyTemperatureC)
                .GreaterThan(35)
                .WithMessage(ExceptionKey.INVALID_TEMPERATURE.ToString())
                .LessThanOrEqualTo(42)
                .WithMessage(ExceptionKey.INVALID_TEMPERATURE.ToString());

            RuleFor(x => x.BloodPressureSystolic)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_BLOOD_PRESSURE_SYSTOLIC.ToString())
                .LessThanOrEqualTo(250)
                .WithMessage(ExceptionKey.INVALID_BLOOD_PRESSURE_SYSTOLIC.ToString());

            RuleFor(x => x.BloodPressureDiastolic)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_BLOOD_PRESSURE_DIASTOLIC.ToString())
                .LessThanOrEqualTo(150)
                .WithMessage(ExceptionKey.INVALID_BLOOD_PRESSURE_DIASTOLIC.ToString());

            RuleFor(x => x.ReceptionId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID.ToString());

            RuleFor(x => x)
                .Must(command =>
                {
                    if (command.IsContraindicatedForVaccination && command.IsEligibleForVaccination)
                    {
                        return false;
                    }

                    if (command.IsContraindicatedForVaccination && command.IsVaccinationDeferred)
                    {
                        return false;
                    }

                    if (command.IsEligibleForVaccination && command.IsContraindicatedForVaccination)
                    {
                        return false;
                    }

                    if (command.IsVaccinationDeferred && command.IsEligibleForVaccination)
                    {
                        return false;
                    }

                    if (command.IsReferredToHospital && command.IsEligibleForVaccination)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage(ExceptionKey.INVALID_TEST_RESULT.ToString());

            RuleFor(x => x)
                .Must(command =>
                {
                    if (command.HasSevereFeverAfterPreviousVaccination)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.HasAcuteOrChronicDisease)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.IsOnOrRecentlyEndedCorticosteroids)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.HasAbnormalTemperatureOrVitals)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.HasAbnormalHeartSound)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.HasHeartValveDisorder)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.HasNeurologicalAbnormalities)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    if (command.IsUnderweightBelow2000g)
                    {
                        return !command.IsEligibleForVaccination;
                    }

                    return true;
                })
                .WithMessage(ExceptionKey.INVALID_TEST_RESULT_FOLLOWING_CONTRAINDICATIONS.ToString());
        }
    }
}