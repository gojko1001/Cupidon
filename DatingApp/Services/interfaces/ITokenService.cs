using DatingApp.DTOs;
using DatingApp.Entities;

namespace DatingApp.Services.interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(AppUser user);
        string GenerateRefreshToken(AppUser user);
        Task<RefreshTokenDto> RenewTokens(RefreshTokenDto refreshTokenDto);
    }
}
