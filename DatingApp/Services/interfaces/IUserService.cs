using DatingApp.DTOs;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services.interfaces
{
    public interface IUserService
    {
        Task<MemberDto> GetUser(string username, bool isCurrentUser);
        Task<PagedList<MemberDto>> GetUsers(UserParams userParams);
        Task UpdateUser(MemberUpdateDto memberUpdateDto, string username);
    }
}
