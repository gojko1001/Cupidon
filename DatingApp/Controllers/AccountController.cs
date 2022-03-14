using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(ITokenService tokenService, IMapper mapper,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username already exists!");
            }

            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
                return BadRequest(result.Errors);

            return await GetReturnUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

            if(user == null)
                return Unauthorized("Invalid username or password!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if(!result.Succeeded)
                return Unauthorized("Invalid username or password!");

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
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == User.GetId());
            if (user == null)
                return Unauthorized();
            var result = await _signInManager.CheckPasswordSignInAsync(user, passwordChangeDto.OldPassword, false);
            if (!result.Succeeded)
                return Unauthorized("Old password is incorrect!");

            if (passwordChangeDto.Password != passwordChangeDto.RepeatPassword)
                return BadRequest("New password and repeat password must be the same");
            if (passwordChangeDto.Password == passwordChangeDto.OldPassword)
                return BadRequest("New password can't be same as current");

            var changeResult = await _userManager.ChangePasswordAsync(user, passwordChangeDto.OldPassword, passwordChangeDto.Password);
            if (changeResult.Errors.Any())
                return BadRequest(changeResult.Errors);
            return Ok();
        }

        private async Task<bool> UserExists(string username) => await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());

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
