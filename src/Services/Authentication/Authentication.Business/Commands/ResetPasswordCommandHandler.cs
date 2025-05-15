using BuildingBlocks.Exceptions;

namespace Authentication.Business.Commands
{
    public record ResetPasswordResult(bool IsSuccess, string Message);
    public record ResetPasswordCommand(string Email) : ICommand<ResetPasswordResult>;

    internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(ValidationStrings.INVALID_EMAIL)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_EMAIL);
        }
    }

    internal class ResetPasswordCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
    {
        public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await applicationUserProto.ResetPasswordAsync(command.Adapt<ResetPasswordRequest>(), cancellationToken: cancellationToken);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message);
            }

            return result.Adapt<ResetPasswordResult>();
        }
    }
}
