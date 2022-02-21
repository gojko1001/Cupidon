using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Services.interfaces;
using DatingApp.Utils;
using Microsoft.Extensions.Options;

namespace DatingApp.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloduinary;

        public PhotoService(IOptions<CloudinarySettings> conf)
        {
            var acc = new Account
            (
                conf.Value.CloudName,
                conf.Value.ApiKey,
                conf.Value.ApiSecret
            );

            _cloduinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloduinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloduinary.DestroyAsync(deleteParams);
        }
    }
}
