using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;

namespace DatingApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetAll();
        Task<AppUser> GetUserById(int id, bool isCurrentUser = false);
        Task<AppUser> GetUserByUsername(string username);
        Task<AppUser> GetUserByUsernameIncludeRefreshTokens(string username);
        Task<AppUser> GetUserByPhotoId(int photoId);
        IQueryable<AppUser> GetMembers(UserParams userParams);
        IQueryable<MemberDto> GetMember(string username, bool isCurrentUser);
        public Task<bool> UserExists(string username);
    }
}
