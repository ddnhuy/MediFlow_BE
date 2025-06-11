using BuildingBlocks.Strings.Enums;
using FileStorage.API.Repositories;

namespace FileStorage.API.Queries
{
    public record GetFilesResult(IEnumerable<FileMetaDataSummaryDto> Files);

    public record GetFilesQuery(string? department, FileType? FileType) : IQuery<GetFilesResult>;

    internal class GetFilesQueryHandler : IQueryHandler<GetFilesQuery, GetFilesResult>
    {
        private readonly IFileRepository _fileRepository;

        public GetFilesQueryHandler(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<GetFilesResult> Handle(GetFilesQuery query, CancellationToken cancellationToken)
        {
            var files = await _fileRepository.GetByFilterAsync(query.department, query.FileType);

            return new GetFilesResult(files.Adapt<IEnumerable<FileMetaDataSummaryDto>>());
        }
    }
}
