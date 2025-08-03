namespace Inventory.Application.Medicines.Commands.ReturnMedicineBatch
{
    public class CreateMedicineBatchReturnCommandValidator : AbstractValidator<CreateMedicineBatchReturnCommand>
    {
        public CreateMedicineBatchReturnCommandValidator()
        {
            RuleFor(x => x.ReturnCode)
                .NotNull()
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_RETURN_CODE.ToString());

            RuleFor(x => x.Reason)
                .NotNull()
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_REASON.ToString());

            RuleFor(x => x.ReceiverName)
                .NotNull()
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_RECEIVER_NAME.ToString());

            RuleFor(x => x.ReceiverEmail)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .WithMessage(ExceptionKey.INVALID_EMAIL.ToString());

            RuleFor(x => x.ReceiverPhone)
                .NotNull()
                .NotEmpty()
                .Matches(@"^[0-9]{10,11}$")
                .WithMessage(ExceptionKey.INVALID_PARENT_PHONE_FORMAT.ToString());

            RuleFor(x => x.Details)
                .NotEmpty()
                .WithMessage(ExceptionKey.AT_LEAST_ONE_MEDICINE_BATCH_REQUIRED.ToString());

            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(x => x.MedicineBatchId)
                    .GreaterThan(0)
                    .WithMessage(ExceptionKey.MEDICINE_BATCH_NOT_FOUND.ToString());

                detail.RuleFor(x => x.BatchNumber)
                    .NotNull()
                    .NotEmpty()
                    .MaximumLength(50)
                    .WithMessage(ExceptionKey.REQUIRED_BATCH_NUMBER.ToString());

                detail.RuleFor(x => x.ExpirationDate)
                    .NotNull()
                    .NotEmpty()
                    .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage(ExceptionKey.INVALID_DATE_RANGE.ToString());

                detail.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage(ExceptionKey.QUANTITY_GREATER_THAN_ZERO.ToString());
            });
        }
    }
}
