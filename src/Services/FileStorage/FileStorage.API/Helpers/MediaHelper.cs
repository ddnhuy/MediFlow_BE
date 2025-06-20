using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Globalization;

namespace FileStorage.API.Services
{
    public interface IMediaHelper
    {
        Task<MediaUploadResultDto> UploadImageAsync(IFormFile file, string? folder, string? imageUrl);
        Task DeleteMediaAsync(string imageUrl);
    }

    public sealed class MediaHelper(
        Cloudinary cloudinary) : IMediaHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public async Task<MediaUploadResultDto> UploadImageAsync(IFormFile file, string? folder, string? imageUrl)
        {
            string extension = Path.GetExtension(file.FileName).ToLower(CultureInfo.CurrentCulture);
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException(ExceptionKey.INVALID_IMAGE_FORMAT.ToString());
            }

            var publicId = string.IsNullOrEmpty(imageUrl) ? null : ExtractPublicIdFromUrl(imageUrl);

            await using Stream stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = string.IsNullOrEmpty(publicId) ? Guid.NewGuid().ToString() : publicId.Substring(publicId.LastIndexOf('/')),
                Folder = "mediflow" + (string.IsNullOrEmpty(folder) ? string.Empty : "/" + folder),
                UniqueFilename = false,
                Overwrite = true
            };

            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

            return new MediaUploadResultDto
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };
        }

        public async Task DeleteMediaAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException(ExceptionKey.INVALID_IMAGE_URL.ToString(), nameof(imageUrl));
            }

            var publicId = ExtractPublicIdFromUrl(imageUrl);

            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await cloudinary.DestroyAsync(deleteParams);

            if (deleteResult.Result != "ok")
            {
                throw new InternalServerException(ExceptionKey.UPLOAD_FAILED);
            }
        }

        public static string ExtractPublicIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException(ExceptionKey.INVALID_IMAGE_URL.ToString());

            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath;
            var extension = Path.GetExtension(path);

            var publicId = path.Split(extension).First().Substring(path.LastIndexOf("mediflow"));

            return publicId;
        }
    }
}
