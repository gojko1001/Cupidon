using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils;

namespace DatingApp.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CloudinaryService _cloudinaryService;

        public PhotoService(IUnitOfWork unitOfWork, CloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Photo> AddPhoto(IFormFile file, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(userId, true);

            var result = await _cloudinaryService.AddPhotoAsync(file);
            if (result.Error != null)
                throw new InvalidActionException(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
            {
                return photo;
            }

            throw new InvalidActionException("Error while adding new photo!");
        }

        public async Task SetMainPhoto(int photoId, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(userId, true);
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            
            if(photo == null)
                throw new NotFoundException();
            if (!photo.IsApproved)
                throw new InvalidActionException("Photo has not been approved yet");
            if (photo.IsMain)
                throw new InvalidActionException("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

            if (currentMain != null)
                currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to set main photo");
        }

        public async Task RemovePhoto(int photoId, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(userId, true);
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
                throw new NotFoundException();
            if (photo.IsMain)
                throw new InvalidActionException("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _cloudinaryService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    throw new InvalidActionException(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to delete photo");
        }

        public Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        }

        public async Task ApprovePhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
            if (photo == null)
                throw new NotFoundException("Photo not found");

            photo.IsApproved = true;
            
            var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);
            if(user != null && !user.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            if(await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to approve photo");
        }

        public async Task RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
            if (photo == null)
                throw new NotFoundException("Photo not found");
            if (photo.PublicId != null)
            {
                var result = await _cloudinaryService.DeletePhotoAsync(photo.PublicId);
                if (result.Error == null)
                    _unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
            else
            {
                _unitOfWork.PhotoRepository.RemovePhoto(photo);
            }

            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to delete photo");
        }
    }
}
