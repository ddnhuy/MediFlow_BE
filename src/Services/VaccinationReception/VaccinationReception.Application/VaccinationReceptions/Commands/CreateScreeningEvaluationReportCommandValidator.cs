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
                .WithMessage("Họ tên phụ huynh không được vượt quá 100 ký tự");

            RuleFor(x => x.ParentPhoneNumber)
                .Matches(@"^[0-9]{10,11}$")
                .WithMessage("Số điện thoại phụ huynh không hợp lệ");

            RuleFor(x => x.WeightKg)
                .GreaterThan(0)
                .WithMessage("Cân nặng phải lớn hơn 0")
                .LessThanOrEqualTo(200)
                .WithMessage("Cân nặng không hợp lệ");

            RuleFor(x => x.BodyTemperatureC)
                .GreaterThan(35)
                .WithMessage("Nhiệt độ cơ thể phải lớn hơn 35°C")
                .LessThanOrEqualTo(42)
                .WithMessage("Nhiệt độ cơ thể không hợp lệ");

            RuleFor(x => x.BloodPressureSystolic)
                .GreaterThan(0)
                .WithMessage("Huyết áp tâm thu phải lớn hơn 0")
                .LessThanOrEqualTo(250)
                .WithMessage("Huyết áp tâm thu không hợp lệ");

            RuleFor(x => x.BloodPressureDiastolic)
                .GreaterThan(0)
                .WithMessage("Huyết áp tâm trương phải lớn hơn 0")
                .LessThanOrEqualTo(150)
                .WithMessage("Huyết áp tâm trương không hợp lệ");

            RuleFor(x => x.ReceptionId)
                .GreaterThan(0)
                .WithMessage("Mã tiếp đón không hợp lệ");

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
                .WithMessage("Kết quả đánh giá không hợp lệ");

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
                .WithMessage("Kết quả đánh giá không phù hợp với các chống chỉ định");
        }
    }
}