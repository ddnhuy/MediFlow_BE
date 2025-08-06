using BuildingBlocks.Exceptions;
using Grpc.Core;

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
            var userInfo = await refreshTokenRepository.FindAsync(command.RefreshToken);

            if (userInfo.UserId == -1)
            {
                throw new BadRequestException(ExceptionKey.INVALID_REFRESH_TOKEN);
            }

            var metadata = new Metadata
            {
                { "x-roles", userInfo.Roles }
            };

            var user = await applicationUserProto.GetApplicationUserAsync(
                new GetApplicationUserRequest { Id = userInfo.UserId },
                metadata,
                cancellationToken: cancellationToken);

            var accessToken = tokenProvider.GenerateAccessToken(user, string.Join(",", user.Departments.Select(d => d.NameInEnglish)));

            return new LoginWithRefreshTokenResult(accessToken, command.RefreshToken);
        }
    }
}
