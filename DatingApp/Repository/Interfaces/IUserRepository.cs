using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;

namespace DatingApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser> GetUserByIdAsync(int id, bool isCurrentUser = false);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<AppUser> GetUserByPhotoIdAsync(int photoId);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser);
    }
}
