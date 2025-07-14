using FileStorage.API.Repositories;

namespace FileStorage.API.Queries
{
    public record GetFileByIdResult(FileMetaDataDto FileMetadata);
    public record GetFileByIdQuery(Guid Id) : IQuery<GetFileByIdResult>;

    internal class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, GetFileByIdResult>
    {
        private readonly IFileRepository _fileRepository;
        public GetFileByIdQueryHandler(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }
        public async Task<GetFileByIdResult> Handle(GetFileByIdQuery query, CancellationToken cancellationToken)
        {
            var fileMetadata = await _fileRepository.GetByIdAsync(query.Id);
            if (fileMetadata is null)
            {
                throw new NotFoundException(ExceptionKey.FILE_NOT_FOUND);
            }

            return new GetFileByIdResult(fileMetadata.Adapt<FileMetaDataDto>());
        }
    }
}
