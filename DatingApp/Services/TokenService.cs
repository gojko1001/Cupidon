using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Services.interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _googleSettings;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _googleSettings = config.GetSection("Authentication:Google");
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(AppUser user)
        {
            RefreshToken refreshToken = new()
            {
                Token = GenerateRefreshToken(),
                Expiration = DateTime.UtcNow.AddDays(7)
            };
            if(user.RefreshTokens == null)
                user.RefreshTokens = new List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);
            _userManager.UpdateAsync(user);

            return refreshToken.Token;
        }

        public async Task<RefreshTokenDto> RenewTokens(RefreshTokenDto refreshTokenDto)
        {
            var username = GetUniqueNameFromAccessToken(refreshTokenDto.AccessToken);
            var user = await _userManager.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.UserName == username);
            ValidateRefreshToken(user, refreshTokenDto.RefreshToken);

            var toDelete = user.RefreshTokens.SingleOrDefault(u => u.Token == refreshTokenDto.RefreshToken);
            user.RefreshTokens.Remove(toDelete);

            await _userManager.UpdateAsync(user);

            return new RefreshTokenDto
            {
                AccessToken = await GenerateJwtToken(user),
                RefreshToken = GenerateRefreshToken(user)
            };
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _googleSettings.GetSection("clientId").Value }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception)
            {
                throw new ServerErrorException("Required to login to google first");
            }
        }


        private string GetUniqueNameFromAccessToken(string accessToken)
        {
            var tokenValidationParamters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateLifetime = false,
                IssuerSigningKey = _key
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParamters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token!");
            }

            var userId = principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException($"Missing claim: {ClaimTypes.Name}!");
            }

            return userId;
        }

        private static void ValidateRefreshToken(AppUser user, string refreshToken)
        {
            if (user == null || !user.RefreshTokens.Any(rt => rt.Token == refreshToken))
            {
                throw new SecurityTokenException("Invalid token!");
            }

            var storedRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

            if (DateTime.UtcNow > storedRefreshToken.Expiration)
            {
                throw new SecurityTokenException("Invalid token!");
            }
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
