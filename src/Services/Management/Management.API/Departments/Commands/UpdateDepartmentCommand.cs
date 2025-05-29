using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;
using Management.API.Helpers;

namespace Management.API.Departments.Commands
{
    public record UpdateDepartmentResult(DepartmentDetailDto Department);
    public record UpdateDepartmentCommand(int Id, string Code, string Name, string NameInEnglish, int DepartmentTypeId, bool IsSuspended, bool IsCancelled, int CurrentUserId) : ICommand<UpdateDepartmentResult>;

    internal class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_ID);
            RuleFor(x => x.Code).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_CODE);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_NAME);
            RuleFor(x => x.NameInEnglish).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_NAME_IN_ENGLISH);
            RuleFor(x => x.DepartmentTypeId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_TYPE);
        }
    }

    internal class UpdateDepartmentCommandHandler(
        DepartmentProtoService.DepartmentProtoServiceClient departmentProto)
        : ICommandHandler<UpdateDepartmentCommand, UpdateDepartmentResult>
    {
        public async Task<UpdateDepartmentResult> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(userId: command.CurrentUserId);

            var request = new UpdateDepartmentRequest
            {
                Id = command.Id,
                Code = command.Code,
                Name = command.Name,
                NameInEnglish = command.NameInEnglish,
                DepartmentTypeId = command.DepartmentTypeId,
                IsSuspended = command.IsSuspended,
                IsCancelled = command.IsCancelled
            };

            var result = await departmentProto.UpdateDepartmentAsync(request, metadata, cancellationToken: cancellationToken);

            return new UpdateDepartmentResult(new DepartmentDetailDto
            {
                Id = result.Id,
                Name = result.Name,
                NameInEnglish = result.NameInEnglish,
                Code = result.Code,
                DepartmentType = new DepartmentTypeDetailDto
                {
                    Id = result.DepartmentTypeId,
                    Name = result.DepartmentTypeName,
                    NameInEnglish = result.DepartmentTypeNameInEnglish
                },
                IsSuspended = result.IsSuspended,
                IsCancelled = result.IsCancelled,
                CreatedAt = result.CreatedAt.ToDateTime(),
                LastUpdatedAt = result.LastUpdatedAt.ToDateTime()
            });
        }
    }
}
