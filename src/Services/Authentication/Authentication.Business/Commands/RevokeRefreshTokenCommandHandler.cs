using FluentValidation;

namespace Authentication.Business.Commands
{
    public record RevokeRefreshTokenResult(bool IsSuccess, string Message);
    public record RevokeRefreshTokenCommand(int UserId, string CurrentRefreshToken) : ICommand<RevokeRefreshTokenResult>;

    internal class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USER_ID.ToString());
            RuleFor(x => x.CurrentRefreshToken).NotEmpty().WithMessage(ExceptionKey.REQUIRED_REFRESH_TOKEN.ToString());
        }
    }

    internal class RevokeRefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository)
        : ICommandHandler<RevokeRefreshTokenCommand, RevokeRefreshTokenResult>
    {
        public async Task<RevokeRefreshTokenResult> Handle(RevokeRefreshTokenCommand command, CancellationToken cancellationToken)
        {
            await refreshTokenRepository.DeleteAllForUserAsync(command.UserId, command.CurrentRefreshToken);

            return new RevokeRefreshTokenResult(true, "Xoá toàn bộ phiên đăng nhập khác thành công.");
        }
    }
}
