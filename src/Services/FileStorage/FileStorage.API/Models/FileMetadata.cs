using BuildingBlocks.Strings.Enums;

namespace FileStorage.API.Models
{
    public class FileMetadata
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }

        public string StoragePath { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public FileType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; } = 1;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public int LastUpdatedBy { get; set; } = 1;
    }
}
