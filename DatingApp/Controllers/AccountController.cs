using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = await _userService.Register(registerDto);
            return await GetReturnUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userService.Login(loginDto);
            var returnUser = await GetReturnUserDto(user);
            returnUser.ProfilePhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url;
            return returnUser;
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            refreshTokenDto.AccessToken = HttpContext.Request.Headers.Authorization.ToString().Substring(7);
            return Ok(await _tokenService.RenewTokens(refreshTokenDto));
        }

        [Authorize]
        [HttpPut("update-password")]
        public async Task<ActionResult> ChangePassword(PasswordChangeDto passwordChangeDto)
        {
            await _userService.ChangePassword(passwordChangeDto, User.GetId());
            return Ok();
        }

        private async Task<UserDto> GetReturnUserDto(AppUser user)
            => new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.GenerateJwtToken(user),
                RefreshToken = _tokenService.GenerateRefreshToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
    }
}
