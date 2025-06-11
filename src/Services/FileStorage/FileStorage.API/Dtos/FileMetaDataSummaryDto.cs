namespace FileStorage.API.Dtos
{
    public class FileMetaDataSummaryDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
