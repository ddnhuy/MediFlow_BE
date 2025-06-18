using Management.API.Helpers;

namespace Management.API.Users.Commands
{
    public record DeleteUserResult(bool IsSuccess, string Message);
    public record DeleteUserCommand(int Id, int CurrentUserId) : ICommand<DeleteUserResult>;

    internal class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(ExceptionKey.REQUIRED_USER_ID.ToString());
        }
    }

    internal class DeleteUserCommandHandler(
        ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        : ICommandHandler<DeleteUserCommand, DeleteUserResult>
    {
        public async Task<DeleteUserResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(userId: command.CurrentUserId);

            var request = new DeleteApplicationUserRequest
            {
                Id = command.Id
            };

            var result = await applicationUserProto.DeleteApplicationUserAsync(request, metadata, cancellationToken: cancellationToken);

            if (result.IsSuccess)
            {
                return new DeleteUserResult(true, $"Xóa người dùng với ID \"{command.Id}\" thành công.");
            }
            else
            {
                return new DeleteUserResult(false, $"Xóa người dùng với ID \"{command.Id}\" thất bại.");
            }
        }
    }
}
