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
                .WithMessage("Mã tiếp đón không hợp lệ");

            RuleFor(x => x.VaccineId)
                .GreaterThan(0)
                .WithMessage("Mã vaccine không hợp lệ");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Số lượng phải lớn hơn 0");

            RuleFor(x => x.ScheduledDate)
                .NotEmpty()
                .WithMessage("Ngày lên lịch không được để trống")
                .Must(date => date > DateTime.Now)
                .WithMessage("Ngày lên lịch phải lớn hơn thời gian hiện tại");


            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Ngày hẹn không được để trống")
                .Must(date => date > DateTime.Now)
                .WithMessage("Ngày hẹn phải lớn hơn thời gian hiện tại");

            RuleFor(x => x.DoctorId)
                .GreaterThan(0)
                .WithMessage("Mã bác sĩ không hợp lệ");

            // Validate Note if provided
            When(x => !string.IsNullOrEmpty(x.Note), () =>
            {
                RuleFor(x => x.Note)
                    .MaximumLength(500)
                    .WithMessage("Ghi chú không được vượt quá 500 ký tự");
            });

            When(x => !string.IsNullOrEmpty(x.TestResultEntry), () =>
            {
                RuleFor(x => x.TestResultEntry)
                    .MaximumLength(1000)
                    .WithMessage("Kết quả xét nghiệm không được vượt quá 1000 ký tự");
            });

            RuleFor(x => x)
                .Must(command =>
                {
                    if (command.IsPaid && command.InvoiceDate == default)
                    {
                        return false;
                    }

                    if (command.IsConfirmed && command.AppointmentDate == default)
                    {
                        return false;
                    }

                    if (command.IsReadyToUse && command.ScheduledDate == default)
                    {
                        return false;
                    }

                    return true;
                })
                .WithMessage("Dữ liệu không hợp lệ theo quy tắc nghiệp vụ");
        }
    }
}