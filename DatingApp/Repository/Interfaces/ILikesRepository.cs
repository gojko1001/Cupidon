using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils;

namespace DatingApp.Repository.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(string username);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
