using Management.API.Helpers;

namespace Management.API.Departments.Commands
{
    public record DeleteDepartmentResult(bool IsSuccess, string Message);
    public record DeleteDepartmentCommand(int Id, int CurrentUserId) : ICommand<DeleteDepartmentResult>;

    internal class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
    {
        public DeleteDepartmentCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_ID);
        }
    }

    internal class DeleteDepartmentCommandHandler(
        DepartmentProtoService.DepartmentProtoServiceClient departmentProto)
        : ICommandHandler<DeleteDepartmentCommand, DeleteDepartmentResult>
    {
        public async Task<DeleteDepartmentResult> Handle(DeleteDepartmentCommand command, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(userId: command.CurrentUserId);

            var request = new DeleteDepartmentRequest
            {
                Id = command.Id
            };

            var result = await departmentProto.DeleteDepartmentAsync(request, metadata, cancellationToken: cancellationToken);

            if (result.IsSuccess)
            {
                return new DeleteDepartmentResult(true, $"Xóa phòng ban với ID \"{command.Id}\" thành công.");
            }
            else
            {
                return new DeleteDepartmentResult(false, $"Xóa phòng ban với ID \"{command.Id}\" thất bại.");
            }
        }
    }
}
