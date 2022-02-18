using DatingApp.Entities;

namespace DatingApp.Services.interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
