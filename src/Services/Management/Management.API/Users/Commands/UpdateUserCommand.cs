using Management.API.Dtos.User;
using Management.API.Helpers;

namespace Management.API.Users.Commands
{
    public record UpdateUserResult(UserDetailDto User);
    public record UpdateUserCommand(int Id, string UserName, string Email, string PhoneNumber, string Code, string Name, string Address, string ProfilePictureUrl, List<string> RoleNames, List<int> DepartmentIds, bool IsSuspended, int CurrentUserId) : ICommand<UpdateUserResult>;

    internal class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USER_ID.ToString());
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(ExceptionKey.REQUIRED_PHONE.ToString());
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_EMAIL.ToString())
                .EmailAddress().WithMessage(ExceptionKey.INVALID_EMAIL.ToString());
            RuleFor(x => x.UserName).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USERNAME.ToString());
            RuleFor(x => x.Code).NotEmpty().WithMessage(ExceptionKey.REQUIRED_CODE.ToString());
            RuleFor(x => x.RoleNames)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_ROLE.ToString())
                .Must(x => x.Count > 0).WithMessage(ExceptionKey.REQUIRED_ROLE.ToString());
            RuleFor(x => x.Name).NotEmpty().WithMessage(ExceptionKey.REQUIRED_NAME.ToString());
            RuleFor(x => x.Address).NotEmpty().WithMessage(ExceptionKey.REQUIRED_ADDRESS.ToString());
            RuleFor(x => x.DepartmentIds)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_DEPARTMENT_IDS.ToString())
                .Must(x => x.Count > 0).WithMessage(ExceptionKey.REQUIRED_DEPARTMENT_IDS.ToString());
        }
    }

    internal class UpdateUserCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        : ICommandHandler<UpdateUserCommand, UpdateUserResult>
    {
        public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(userId: command.CurrentUserId);

            var request = new UpdateApplicationUserRequest
            {
                Id = command.Id,
                UserName = command.UserName,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber,
                Code = command.Code,
                Name = command.Name,
                Address = command.Address,
                ProfilePictureUrl = command.ProfilePictureUrl,
                RoleNames = { command.RoleNames },
                DepartmentIds = { command.DepartmentIds },
                IsSuspended = command.IsSuspended
            };

            var result = await applicationUserProto.UpdateApplicationUserAsync(request, metadata, cancellationToken: cancellationToken);

            return new UpdateUserResult(ConvertUserHelper.ToUserDetailDto(result));
        }
    }
}
