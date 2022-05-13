using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;
using Google.Apis.Auth;

namespace DatingApp.Services.interfaces
{
    public interface IUserService
    {
        Task<MemberDto> GetUser(string username, string requestingUser);
        Task<PagedList<MemberDto>> GetUsers(UserParams userParams);
        Task<IEnumerable<object>> GetUsersWithRole();
        Task UpdateUser(MemberUpdateDto memberUpdateDto, string username);
        Task ChangePassword(PasswordChangeDto passwordChangeDto, int userId);
        Task<IEnumerable<string>> EditRoles(string username, string[] roles);
        Task<AppUser> Register(RegisterDto registerDto);
        Task<AppUser> Login(LoginDto loginDto);
        Task<AppUser> LoginGoogle(ExternalAuthDto externalAuthDto, GoogleJsonWebSignature.Payload payload);
    }
}
