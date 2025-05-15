namespace Authentication.Business.Commands
{
    public record LoginWithRefreshTokenResult(string AccessToken, string RefreshToken);
    public record LoginWithRefreshTokenCommand(string RefreshToken) : ICommand<LoginWithRefreshTokenResult>;

    internal class LoginWithRefreshTokenCommandValidator : AbstractValidator<LoginWithRefreshTokenCommand>
    {
        public LoginWithRefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ValidationStrings.REQUIRED_REFRESH_TOKEN);
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
                throw new InvalidRefreshTokenException();
            }

            var user = await applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest { Id = userId }, cancellationToken: cancellationToken);

            var accessToken = tokenProvider.GenerateAccessToken(user, string.Join(",", user.Departments.Select(d => d.Name)));

            return new LoginWithRefreshTokenResult(accessToken, command.RefreshToken);
        }
    }
}
