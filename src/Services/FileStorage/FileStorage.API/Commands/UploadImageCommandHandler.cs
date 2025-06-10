namespace FileStorage.API.Commands
{
    public record UploadImageResult(string ImageUrl);
    public record UploadImageCommand(IFormFile File, string? Folder, string? ImageUrl) : ICommand<UploadImageResult>;

    internal class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
    {
        public UploadImageCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage(ValidationStrings.FILE_NOT_PROVIDED)
                .Must(file => file.Length > 0).WithMessage(ValidationStrings.FILE_NOT_PROVIDED)
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage(ValidationStrings.FILE_TOO_LARGE);
            RuleFor(x => x.Folder)
                .MaximumLength(100).WithMessage(ValidationStrings.FOLDER_NAME_TOO_LONG);
            RuleFor(x => x.ImageUrl)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage(ValidationStrings.INVALID_IMAGE_URL);
        }
    }

    public class UploadImageCommandHandler(
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
