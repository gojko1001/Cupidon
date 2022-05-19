using CloudinaryDotNet.Actions;

namespace DatingApp.Utils.CloudinaryUtil
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> AddPhoto(IFormFile file);
        Task<DeletionResult> DeletePhoto(string publicId);
        Task DeletePhotos(IEnumerable<string> photoIds);
    }
}
