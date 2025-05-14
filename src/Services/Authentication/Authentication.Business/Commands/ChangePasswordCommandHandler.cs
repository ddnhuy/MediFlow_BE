using BuildingBlocks.Exceptions;

namespace Authentication.Business.Commands
{
    public record ChangePasswordResult(bool IsSuccess, string Message);
    public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword) : ICommand<ChangePasswordResult>;

    internal class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(ValidationStrings.REQUIRED_USER_ID);
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_CURRENT_PASSWORD)
                .MinimumLength(8).WithMessage(ValidationStrings.INVALID_PASSWORD_LENGTH);
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_NEW_PASSWORD)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$").WithMessage(ValidationStrings.INVALID_PASSWORD_LENGTH)
                .MinimumLength(8).WithMessage(ValidationStrings.INVALID_PASSWORD_LENGTH)
                .NotEqual(x => x.CurrentPassword).WithMessage(ValidationStrings.INVALID_NEW_PASSWORD);
        }
    }

    internal class ChangePasswordCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : ICommandHandler<ChangePasswordCommand, ChangePasswordResult>
    {
        public async Task<ChangePasswordResult> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await applicationUserProto.ChangePasswordAsync(command.Adapt<ChangePasswordRequest>(), cancellationToken: cancellationToken);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message);
            }

            return result.Adapt<ChangePasswordResult>();
        }
    }
}
