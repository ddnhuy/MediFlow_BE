using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;
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
            RuleFor(x => x.UserName).NotEmpty().WithMessage(ValidationStrings.REQUIRED_USERNAME);
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_EMAIL)
                .EmailAddress().WithMessage(ValidationStrings.INVALID_EMAIL);
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_PASSWORD)
                .MinimumLength(8).WithMessage(ValidationStrings.INVALID_PASSWORD_LENGTH);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(ValidationStrings.REQUIRED_PHONE);
            RuleFor(x => x.Code).NotEmpty().WithMessage(ValidationStrings.REQUIRED_CODE);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationStrings.REQUIRED_NAME);
            RuleFor(x => x.RoleNames)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_ROLE)
                .Must(x => x.Count > 0).WithMessage(ValidationStrings.REQUIRED_ROLE);
            RuleFor(x => x.DepartmentIds)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_IDS)
                .Must(x => x.Count > 0).WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_IDS);
            RuleFor(x => x.Address).NotEmpty().WithMessage(ValidationStrings.REQUIRED_ADDRESS);
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
