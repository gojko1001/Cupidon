using DatingApp.Data;
using DatingApp.Entities;
using DatingApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _context;

        public RefreshTokenRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<RefreshToken>> GetExpiredTokens()
        {
            return await _context.RefreshTokens.Where(rt => rt.Expiration.CompareTo(DateTime.UtcNow) < 0).ToListAsync();
        }

        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = await GetExpiredTokens();
            _context.RefreshTokens.RemoveRange(expiredTokens);
        }
    }
}
