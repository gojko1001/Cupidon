using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils;

namespace DatingApp.Services.interfaces
{
    public interface IUserService
    {
        Task<MemberDto> GetUser(string username, bool isCurrentUser);
        Task<PagedList<MemberDto>> GetUsers(UserParams userParams);
        Task<Photo> AddPhoto(IFormFile file, int userId);
        Task SetMainPhoto(int photoId, int userId);
        Task RemovePhoto(int photoId);
        Task UpdateUser(MemberUpdateDto memberUpdateDto, string username);
    }
}
