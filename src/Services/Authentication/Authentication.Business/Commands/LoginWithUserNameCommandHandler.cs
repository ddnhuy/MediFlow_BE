using BuildingBlocks.Exceptions;
using FluentValidation;
using Grpc.Core;

namespace Authentication.Business.Commands
{
    public record LoginWithUserNameResult(string AccessToken, string RefreshToken);
    public record LoginWithUserNameCommand(string UserName, string Password) : ICommand<LoginWithUserNameResult>;

    internal class LoginWithUserNameCommandValidator : AbstractValidator<LoginWithUserNameCommand>
    {
        public LoginWithUserNameCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USERNAME.ToString());
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_PASSWORD.ToString())
                .MinimumLength(8).WithMessage(ExceptionKey.INVALID_PASSWORD_LENGTH.ToString());
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
                throw new BadRequestException(ExceptionKey.INVALID_LOGIN_CREDENTIAL);
            }

            var accessToken = tokenProvider.GenerateAccessToken(loginResponse.User, string.Join(",", loginResponse.User.Departments.Select(d => d.NameInEnglish)));
            var refreshToken = tokenProvider.GenerateRefreshToken();

            await refreshTokenRepository.AddAsync(refreshToken, loginResponse.User.Id, loginResponse.User.Roles);

            return new LoginWithUserNameResult(accessToken, refreshToken);
        }
    }
}
