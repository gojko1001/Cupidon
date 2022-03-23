using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public Task<MemberDto> GetUser(string username, bool isCurrentUser)
        {
            return _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser);
        }

        public async Task<PagedList<MemberDto>> GetUsers(UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(userParams.CurrentUsername);
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            return await _unitOfWork.UserRepository.GetMembersAsync(userParams);
        }

        public async Task<IEnumerable<object>> GetUsersWithRole()
        {
            return await _userManager.Users
                        .Include(r => r.Roles).ThenInclude(r => r.Role)
                        .OrderBy(u => u.UserName)
                        .Select(u => new
                        {
                            u.Id,
                            Username = u.UserName,
                            Roles = u.Roles.Select(r => r.Role.Name).ToList()
                        })
                        .ToListAsync();
        }

        public async Task UpdateUser(MemberUpdateDto memberUpdateDto, string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to update user info");
        }
        
        public async Task<IEnumerable<string>> EditRoles(string username, string[] roles)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new InvalidActionException("User doesn't exist");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, roles.Except(userRoles));
            if (!result.Succeeded)
                throw new InvalidActionException("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(roles));
            if (!result.Succeeded)
                throw new InvalidActionException("Failed to remove from roles");

            return await _userManager.GetRolesAsync(user);
        }

        public async Task ChangePassword(PasswordChangeDto passwordChangeDto, int userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new UnauthorizedException();
            var result = await _signInManager.CheckPasswordSignInAsync(user, passwordChangeDto.OldPassword, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Old password is incorrect!");

            if (passwordChangeDto.Password != passwordChangeDto.RepeatPassword)
                throw new InvalidActionException("New password and repeat password must be the same");
            if (passwordChangeDto.Password == passwordChangeDto.OldPassword)
                throw new InvalidActionException("New password can't be same as current");

            var changeResult = await _userManager.ChangePasswordAsync(user, passwordChangeDto.OldPassword, passwordChangeDto.Password);
            if (changeResult.Errors.Any())
                throw new InvalidActionException(string.Join(Environment.NewLine, changeResult.Errors.Select(e => e.Description)));
        }

        public async Task<AppUser> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                throw new InvalidActionException("Username already exists!");
            }

            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new InvalidActionException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
                throw new InvalidActionException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

            return user;
        }

        public async Task<AppUser> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

            if (user == null)
                throw new UnauthorizedException("Invalid username or password!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid username or password!");

            return user;
        }


        private async Task<bool> UserExists(string username) => await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
