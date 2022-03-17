using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils;

namespace DatingApp.Services
{
    public class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddLike(string sourceUsername, string likedUsername)
        {
            var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUsername);
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(likedUsername);

            if (likedUser == null)
                throw new NotFoundException();

            var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUser.Id, likedUser.Id);
            if (userLike != null)
                throw new InvalidActionException("You already liked this user!");

            userLike = new UserLike
            {
                SourceUserId = sourceUser.Id,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to like user!");
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            return await _unitOfWork.LikesRepository.GetUserLikes(likesParams);
        }
    }
}
