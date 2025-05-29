using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;
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
            RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationStrings.REQUIRED_USER_ID);
            RuleFor(x => x.UserName).NotEmpty().WithMessage(ValidationStrings.REQUIRED_USERNAME);
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_EMAIL)
                .EmailAddress().WithMessage(ValidationStrings.INVALID_EMAIL);
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

            return new UpdateUserResult(new UserDetailDto
            {
                Id = result.Id,
                UserName = result.UserName,
                Email = result.Email,
                Code = result.Code,
                Name = result.Name,
                PhoneNumber = result.PhoneNumber,
                EmailConfirmed = result.EmailConfirmed,
                PhoneNumberConfirmed = result.PhoneNumberConfirmed,
                TwoFactorEnabled = result.TwoFactorEnabled,
                IsSuspended = result.IsSuspended,
                IsCancelled = result.IsCancelled,
                CreatedAt = result.CreatedAt.ToDateTime(),
                LastUpdatedAt = result.LastUpdatedAt.ToDateTime(),
                Roles = result.Roles.Split(',').ToList(),
                Departments = result.Departments.Select(d => new DepartmentSummaryDto
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    NameInEnglish = d.NameInEnglish,
                    DepartmentType = new DepartmentTypeSummaryDto
                    {
                        Name = d.DepartmentTypeName,
                        NameInEnglish = d.DepartmentTypeNameInEnglish
                    },
                    IsSuspended = d.IsSuspended
                }).ToList(),
                Address = result.Address,
                ProfilePictureUrl = result.ProfilePictureUrl
            });
        }
    }
}
