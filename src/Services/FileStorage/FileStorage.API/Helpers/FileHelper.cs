using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace FileStorage.API.Helpers
{
    public interface IFileHelper
    {
        Task UploadFileAsync(string key, IFormFile file, string folder);
        Task<string> GenerateDownloadUrl(string key, int expiresMinutes = 10);
        Task DeleteFileAsync(string key);
    }

    public class FileHelper : IFileHelper
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;

        public FileHelper(IConfiguration config)
        {
            _config = config;

            _s3Client = new AmazonS3Client(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                RegionEndpoint.GetBySystemName(config["AWS:Region"])
            );
        }

        public async Task UploadFileAsync(string key, IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = _config["AWS:BucketName"],
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(request);
        }

        public Task<string> GenerateDownloadUrl(string key, int expiresMinutes = 10)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _config["AWS:BucketName"],
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes)
            };
            return _s3Client.GetPreSignedURLAsync(request);
        }

        public async Task DeleteFileAsync(string key)
        {
            await _s3Client.DeleteObjectAsync(_config["AWS:BucketName"], key);
        }
    }
}
