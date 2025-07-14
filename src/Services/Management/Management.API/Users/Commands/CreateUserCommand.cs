using Management.API.Dtos.User;
using Management.API.Helpers;

namespace Management.API.Users.Commands
{
    public record CreateUserResult(UserDetailDto User);
    public record CreateUserCommand(string UserName, string Email, string Password, string PhoneNumber, string Code, string Name, string Address, string ProfilePictureUrl, List<string> RoleNames, List<int> DepartmentIds, int CurrentUserId) : ICommand<CreateUserResult>;

    internal class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USERNAME.ToString());
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_EMAIL.ToString())
                .EmailAddress().WithMessage(ExceptionKey.INVALID_EMAIL.ToString());
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_PASSWORD.ToString())
                .MinimumLength(8).WithMessage(ExceptionKey.INVALID_PASSWORD_LENGTH.ToString());
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(ExceptionKey.REQUIRED_PHONE.ToString());
            RuleFor(x => x.Code).NotEmpty().WithMessage(ExceptionKey.REQUIRED_CODE.ToString());
            RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionKey.REQUIRED_NAME.ToString());
            RuleFor(x => x.RoleNames)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_ROLE.ToString())
                .Must(x => x.Count > 0).WithMessage(ExceptionKey.REQUIRED_ROLE.ToString());
            RuleFor(x => x.DepartmentIds)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_DEPARTMENT_IDS.ToString())
                .Must(x => x.Count > 0).WithMessage(ExceptionKey.REQUIRED_DEPARTMENT_IDS.ToString());
            RuleFor(x => x.Address).NotEmpty().WithMessage(ExceptionKey.REQUIRED_ADDRESS.ToString());
        }
    }

    internal class CreateUserCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        : ICommandHandler<CreateUserCommand, CreateUserResult>
    {
        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(userId: command.CurrentUserId);

            var request = new CreateApplicationUserRequest
            {
                UserName = command.UserName,
                Email = command.Email,
                Password = command.Password,
                PhoneNumber = command.PhoneNumber,
                Code = command.Code,
                Name = command.Name,
                Address = command.Address,
                ProfilePictureUrl = command.ProfilePictureUrl,
                RoleNames = { command.RoleNames },
                DepartmentIds = { command.DepartmentIds }
            };

            var result = await applicationUserProto.CreateApplicationUserAsync(request, metadata, cancellationToken: cancellationToken);

            return new CreateUserResult(ConvertUserHelper.ToUserDetailDto(result));
        }
    }
}
