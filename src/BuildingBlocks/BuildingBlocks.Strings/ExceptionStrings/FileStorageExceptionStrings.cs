namespace BuildingBlocks.Strings.ExceptionStrings
{
    public static class FileStorageExceptionStrings
    {
        public const string UPLOAD_FAILED = "Chúng tôi gặp vấn đề khi đang cố gắng xoá thông tin của bạn. Vui lòng thử lại.";
        public static string FILE_NOT_FOUND(Guid id) => $"Không tìm thấy tệp với ID {id}.";
    }
}
