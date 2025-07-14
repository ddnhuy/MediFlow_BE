using BuildingBlocks.Exceptions;
using FluentValidation;

namespace Authentication.Business.Commands
{
    public record ResetPasswordResult(bool IsSuccess, string Message);
    public record ResetPasswordCommand(string Email) : ICommand<ResetPasswordResult>;

    internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage(ExceptionKey.INVALID_EMAIL.ToString())
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_EMAIL.ToString());
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
                throw new BadRequestException(ExceptionKey.FAILED_RESET_PASSWORD);
            }

            return result.Adapt<ResetPasswordResult>();
        }
    }
}
