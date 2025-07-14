using BuildingBlocks.Exceptions;
using FluentValidation;
using Grpc.Core;

namespace Authentication.Business.Commands
{
    public record ChangePasswordResult(bool IsSuccess, string Message);
    public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword, string ChangerId) : ICommand<ChangePasswordResult>;

    internal class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USER_ID.ToString());
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_CURRENT_PASSWORD.ToString())
                .MinimumLength(8).WithMessage(ExceptionKey.INVALID_PASSWORD_LENGTH.ToString());
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_NEW_PASSWORD.ToString())
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$").WithMessage(ExceptionKey.INVALID_PASSWORD_LENGTH.ToString())
                .MinimumLength(8).WithMessage(ExceptionKey.INVALID_PASSWORD_LENGTH.ToString())
                .NotEqual(x => x.CurrentPassword).WithMessage(ExceptionKey.INVALID_NEW_PASSWORD.ToString());
        }
    }

    internal class ChangePasswordCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto) : ICommandHandler<ChangePasswordCommand, ChangePasswordResult>
    {
        public async Task<ChangePasswordResult> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var metadata = new Metadata
            {
                { "x-user-id", command.ChangerId }
            };
            var result = await applicationUserProto.ChangePasswordAsync(command.Adapt<ChangePasswordRequest>(), metadata, cancellationToken: cancellationToken);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(ExceptionKey.FAILED_CHANGE_PASSWORD);
            }

            return result.Adapt<ChangePasswordResult>();
        }
    }
}
