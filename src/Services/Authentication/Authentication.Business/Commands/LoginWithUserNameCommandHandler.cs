namespace Authentication.Business.Commands
{
    public record LoginWithUserNameResult(string AccessToken, string RefreshToken);
    public record LoginWithUserNameCommand(string UserName, string Password) : ICommand<LoginWithUserNameResult>;

    internal class LoginWithUserNameCommandValidator : AbstractValidator<LoginWithUserNameCommand>
    {
        public LoginWithUserNameCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(ValidationStrings.REQUIRED_USERNAME);
            RuleFor(x => x.Password).NotEmpty().WithMessage(ValidationStrings.REQUIRED_PASSWORD);
        }
    }

    internal class LoginWithUserNameCommandHandler(
        ITokenProvider tokenProvider,
        IRefreshTokenRepository refreshTokenRepository,
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        : ICommandHandler<LoginWithUserNameCommand, LoginWithUserNameResult>
    {
        public async Task<LoginWithUserNameResult> Handle(LoginWithUserNameCommand command, CancellationToken cancellationToken)
        {
            var loginResponse = await applicationUserProto.LoginAsync(command.Adapt<LoginRequest>(), cancellationToken: cancellationToken);

            if (!loginResponse.IsSuccess)
            {
                throw new InvalidLoginException(loginResponse.Message);
            }

            var accessToken = tokenProvider.GenerateAccessToken(loginResponse.User, string.Join(",", loginResponse.User.Departments.Select(d => d.Name)));
            var refreshToken = tokenProvider.GenerateRefreshToken();

            _ = refreshTokenRepository.AddAsync(refreshToken, loginResponse.User.Id);

            return new LoginWithUserNameResult(accessToken, refreshToken);
        }
    }
}
