using Management.API.Dtos.Department;
using Management.API.Dtos.DepartmentType;
using Management.API.Helpers;

namespace Management.API.Departments.Commands
{
    public record CreateDepartmentResult(DepartmentDetailDto Department);
    public record CreateDepartmentCommand(string Code, string Name, string NameInEnglish, int DepartmentTypeId, int CurrentUserId) : ICommand<CreateDepartmentResult>;

    internal class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_CODE);
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_NAME);
            RuleFor(x => x.NameInEnglish).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_NAME_IN_ENGLISH);
            RuleFor(x => x.DepartmentTypeId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_TYPE);
        }
    }

    internal class CreateDepartmentCommandHandler(
        DepartmentProtoService.DepartmentProtoServiceClient departmentProto)
        : ICommandHandler<CreateDepartmentCommand, CreateDepartmentResult>
    {
        public async Task<CreateDepartmentResult> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(userId: command.CurrentUserId);

            var request = new CreateDepartmentRequest
            {
                Code = command.Code,
                Name = command.Name,
                NameInEnglish = command.NameInEnglish,
                DepartmentTypeId = command.DepartmentTypeId
            };

            var result = await departmentProto.CreateDepartmentAsync(request, metadata, cancellationToken: cancellationToken);

            return new CreateDepartmentResult(new DepartmentDetailDto
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
