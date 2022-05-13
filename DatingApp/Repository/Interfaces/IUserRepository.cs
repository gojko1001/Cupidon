using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;

namespace DatingApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<AppUser> GetUserById(int id, bool isCurrentUser = false);
        Task<AppUser> GetUserByUsername(string username);
        Task<AppUser> GetUserByUsernameIncludeRefreshTokens(string username);
        Task<AppUser> GetUserByEmail(string email);
        Task<AppUser> GetUserByPhotoId(int photoId);
        IQueryable<MemberDto> GetMembers(UserParams userParams);
        IQueryable<MemberDto> GetMember(string username, bool isCurrentUser);
    }
}
