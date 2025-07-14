namespace FileStorage.API.Commands
{
    public record UploadImageResult(string ImageUrl);
    public record UploadImageCommand(IFormFile File, string? Folder, string? ImageUrl) : ICommand<UploadImageResult>;

    internal class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
    {
        public UploadImageCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage(ExceptionKey.FILE_NOT_PROVIDED.ToString())
                .Must(file => file.Length > 0).WithMessage(ExceptionKey.FILE_NOT_PROVIDED.ToString())
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage(ExceptionKey.FILE_TOO_LARGE.ToString());
            RuleFor(x => x.Folder)
                .MaximumLength(100).WithMessage(ExceptionKey.FOLDER_NAME_TOO_LONG.ToString());
            RuleFor(x => x.ImageUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage(ExceptionKey.INVALID_IMAGE_URL.ToString());
        }
    }

    internal class UploadImageCommandHandler(
        IMediaHelper mediaHelper)
        : ICommandHandler<UploadImageCommand, UploadImageResult>
    {
        public async Task<UploadImageResult> Handle(UploadImageCommand command, CancellationToken cancellationToken)
        {
            MediaUploadResultDto result = await mediaHelper.UploadImageAsync(command.File, command.Folder, command.ImageUrl);
            return new UploadImageResult(result.Url);
        }
    }
}
