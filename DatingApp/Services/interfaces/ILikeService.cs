using DatingApp.DTOs;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services.interfaces
{
    public interface ILikeService
    {
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
        Task AddLike(string sourceUsername, string likedUsername);
    }
}
