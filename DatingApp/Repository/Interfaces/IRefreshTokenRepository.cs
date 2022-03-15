using DatingApp.Entities;

namespace DatingApp.Repository.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<IEnumerable<RefreshToken>> GetExpiredTokens();
        Task RemoveExpiredTokensAsync();
    }
}
