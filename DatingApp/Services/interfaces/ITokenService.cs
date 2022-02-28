using DatingApp.Entities;

namespace DatingApp.Services.interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
