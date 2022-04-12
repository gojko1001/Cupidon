using DatingApp.DTOs;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services.interfaces
{
    public interface IUserRelationService
    {
        Task<PagedList<RelationDto>> GetUserRelations(RelationParams relationParams);
        Task AddLike(string sourceUsername, string likedUsername);
    }
}
