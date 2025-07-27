using BuildingBlocks.Exceptions;
using FluentValidation;
using Grpc.Core;

namespace Authentication.Business.Commands
{
    public record ConfirmPasswordResult(bool IsSuccess, string Message);
    public record ConfirmPasswordCommand(int UserId, string Password) : ICommand<ConfirmPasswordResult>;

    internal class ConfirmPasswordCommandValidator : AbstractValidator<ConfirmPasswordCommand>
    {
        public ConfirmPasswordCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USER_ID.ToString());
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_PASSWORD.ToString())
                .MinimumLength(8).WithMessage(ExceptionKey.INVALID_PASSWORD_LENGTH.ToString());
        }
    }

    internal class ConfirmPasswordCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        : ICommandHandler<ConfirmPasswordCommand, ConfirmPasswordResult>
    {
        public async Task<ConfirmPasswordResult> Handle(ConfirmPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await applicationUserProto.ConfirmPasswordAsync(
                new ConfirmPasswordRequest
                {
                    UserId = command.UserId,
                    Password = command.Password
                },
                cancellationToken: cancellationToken);

            return new ConfirmPasswordResult(result.IsSuccess, result.Message);
        }
    }
}