using BuildingBlocks.Exceptions;
using FluentValidation;

namespace Authentication.Business.Commands
{
    public record LoginWithRefreshTokenResult(string AccessToken, string RefreshToken);
    public record LoginWithRefreshTokenCommand(string RefreshToken) : ICommand<LoginWithRefreshTokenResult>;

    internal class LoginWithRefreshTokenCommandValidator : AbstractValidator<LoginWithRefreshTokenCommand>
    {
        public LoginWithRefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ExceptionKey.REQUIRED_REFRESH_TOKEN.ToString());
        }
    }

    internal class LoginWithRefreshTokenCommandHandler(
        ITokenProvider tokenProvider,
        IRefreshTokenRepository refreshTokenRepository,
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        : ICommandHandler<LoginWithRefreshTokenCommand, LoginWithRefreshTokenResult>
    {
        public async Task<LoginWithRefreshTokenResult> Handle(LoginWithRefreshTokenCommand command, CancellationToken cancellationToken)
        {
            int userId = await refreshTokenRepository.FindAsync(command.RefreshToken);

            if (userId == -1)
            {
                throw new BadRequestException(ExceptionKey.INVALID_REFRESH_TOKEN);
            }

            var user = await applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest { Id = userId }, cancellationToken: cancellationToken);

            var accessToken = tokenProvider.GenerateAccessToken(user, string.Join(",", user.Departments.Select(d => d.NameInEnglish)));

            return new LoginWithRefreshTokenResult(accessToken, command.RefreshToken);
        }
    }
}
