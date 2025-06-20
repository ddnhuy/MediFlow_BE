using FileStorage.API.Helpers;
using FileStorage.API.Repositories;
using FluentValidation;

namespace FileStorage.API.Commands
{
    public record DeleteFileResult(bool IsSuccess);
    public record DeleteFileCommand(Guid Id) : ICommand<DeleteFileResult>;

    internal class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
    {
        public DeleteFileCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ExceptionKey.INVALID_FILE_ID.ToString());
        }
    }

    internal class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand, DeleteFileResult>
    {
        private readonly IFileHelper _fileHelper;
        private readonly IFileRepository _fileRepository;

        public DeleteFileCommandHandler(IFileHelper fileHelper, IFileRepository fileRepository)
        {
            _fileHelper = fileHelper;
            _fileRepository = fileRepository;
        }

        public async Task<DeleteFileResult> Handle(DeleteFileCommand command, CancellationToken cancellationToken)
        {
            var fileMetadata = await _fileRepository.GetByIdAsync(command.Id);
            if (fileMetadata is null)
            {
                throw new NotFoundException(ExceptionKey.FILE_NOT_FOUND);
            }

            await _fileHelper.DeleteFileAsync(fileMetadata.StoragePath);
            await _fileRepository.DeleteAsync(fileMetadata);

            return new DeleteFileResult(true);
        }
    }
}
