using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IPhotoService photoService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }


        public Task<MemberDto> GetUser(string username, bool isCurrentUser)
        {
            return _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser);
        }

        public async Task<PagedList<MemberDto>> GetUsers(UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(userParams.CurrentUsername);
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            return await _unitOfWork.UserRepository.GetMembersAsync(userParams);
        }
        
        public async Task UpdateUser(MemberUpdateDto memberUpdateDto, string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to update user info");
        }


        public async Task<Photo> AddPhoto(IFormFile file, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);

            var result = await _photoService.AddPhotoAsync(file);
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
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

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

        public async Task RemovePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByPhotoIdAsync(photoId);

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
                throw new NotFoundException();
            if (photo.IsMain)
                throw new InvalidActionException("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    throw new InvalidActionException(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to delete photo");
        }
    }
}
