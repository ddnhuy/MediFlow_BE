using BuildingBlocks.Strings.Enums;
using FileStorage.API.Helpers;
using FileStorage.API.Models;
using FileStorage.API.Repositories;

namespace FileStorage.API.Commands
{
    public record UploadFileResult(FileMetaDataDto FileMetaData);
    public record UploadFileCommand(IFormFile File, string Department, FileType Type) : ICommand<UploadFileResult>;

    internal class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
    {
        public UploadFileCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage(ValidationStrings.FILE_NOT_PROVIDED)
                .Must(file => file.Length > 0).WithMessage(ValidationStrings.FILE_NOT_PROVIDED)
                .Must(file => file.Length <= 10 * 1024 * 1024).WithMessage(ValidationStrings.FILE_TOO_LARGE(10));
            RuleFor(x => x.Department)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_DEPARTMENT_NAME);
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage(ValidationStrings.INVALID_FILE_TYPE);
        }
    }

    internal class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, UploadFileResult>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileHelper _fileHelper;
        private readonly ICurrentUserHelper _currentUserHelper;

        public UploadFileCommandHandler(IFileRepository fileRepository, IFileHelper fileHelper, ICurrentUserHelper currentUserHelper)
        {
            _fileRepository = fileRepository;
            _fileHelper = fileHelper;
            _currentUserHelper = currentUserHelper;
        }

        public async Task<UploadFileResult> Handle(UploadFileCommand command, CancellationToken cancellationToken)
        {
            var file = command.File;
            var department = command.Department;
            var type = command.Type;
            var folder = type switch
            {
                FileType.Report => "reports",
                FileType.Statistics => "statistics",
                _ => throw new ArgumentOutOfRangeException(nameof(type), ValidationStrings.INVALID_FILE_TYPE)
            };
            var fileKey = $"{folder}/{Guid.NewGuid()}_{file.FileName}";

            var uploadTask = _fileHelper.UploadFileAsync(fileKey, file, folder);

            var fileMetadata = new FileMetadata
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                StoragePath = fileKey,
                Department = department,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserHelper.GetUserId(),
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = _currentUserHelper.GetUserId()
            };

            await _fileRepository.CreateAsync(fileMetadata);
            await uploadTask;

            return new UploadFileResult(new FileMetaDataDto
            {
                Id = fileMetadata.Id,
                FileName = fileMetadata.FileName,
                ContentType = fileMetadata.ContentType,
                Size = fileMetadata.Size,
                StoragePath = fileMetadata.StoragePath,
                Department = fileMetadata.Department,
                CreatedAt = fileMetadata.CreatedAt,
                LastUpdatedAt = fileMetadata.LastUpdatedAt
            });
        }
    }
}
