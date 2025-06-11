using BuildingBlocks.Strings.ExceptionStrings;
using FileStorage.API.Helpers;
using FileStorage.API.Repositories;

namespace FileStorage.API.Commands
{
    public record CreateFileDowloadUrlResult(string Url);
    public record CreateFileDownloadUrlCommand(Guid Id) : ICommand<CreateFileDowloadUrlResult>;

    internal class CreateFileDownloadUrlCommandValidator : AbstractValidator<CreateFileDownloadUrlCommand>
    {
        public CreateFileDownloadUrlCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidationStrings.INVALID_FILE_ID);
        }
    }

    internal class CreateFileDownloadUrlCommandHandler : ICommandHandler<CreateFileDownloadUrlCommand, CreateFileDowloadUrlResult>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileHelper _fileHelper;

        public CreateFileDownloadUrlCommandHandler(IFileRepository fileRepository, IFileHelper fileHelper)
        {
            _fileRepository = fileRepository;
            _fileHelper = fileHelper;
        }

        public async Task<CreateFileDowloadUrlResult> Handle(CreateFileDownloadUrlCommand command, CancellationToken cancellationToken)
        {
            var fileMetadata = await _fileRepository.GetByIdAsync(command.Id);

            if (fileMetadata == null)
            {
                throw new NotFoundException(FileStorageExceptionStrings.FILE_NOT_FOUND(command.Id));
            }

            var url = await _fileHelper.GenerateDownloadUrl(fileMetadata.StoragePath);

            return new CreateFileDowloadUrlResult(url);
        }
    }
}
