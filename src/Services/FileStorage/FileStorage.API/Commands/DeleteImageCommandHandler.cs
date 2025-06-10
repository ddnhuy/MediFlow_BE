namespace FileStorage.API.Commands
{
    public record DeleteImageResult(bool IsSuccess);
    public record DeleteImageCommand(string ImageUrl) : ICommand<DeleteImageResult>;

    public class DeleteImageCommandHandler : ICommandHandler<DeleteImageCommand, DeleteImageResult>
    {
        private readonly IMediaHelper _mediaHelper;
        public DeleteImageCommandHandler(IMediaHelper mediaHelper)
        {
            _mediaHelper = mediaHelper;
        }
        public async Task<DeleteImageResult> Handle(DeleteImageCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.ImageUrl))
            {
                throw new ArgumentException(ValidationStrings.INVALID_IMAGE_URL, nameof(command.ImageUrl));
            }

            await _mediaHelper.DeleteMediaAsync(command.ImageUrl);
            return new DeleteImageResult(true);
        }
    }
}