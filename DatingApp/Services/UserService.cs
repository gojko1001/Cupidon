using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils.CloudinaryUtil;
using DatingApp.Utils.Pagination;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserRelationService _userRelationService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IUserRelationService userRelationService, ICloudinaryService cloudinaryService,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRelationService = userRelationService;
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<MemberDto> GetUser(string username, string requestingUsername)
        {
            if (username == requestingUsername)
                return await GetSelfInfo(username);
            else
                return await GetRequestedUserInfo(username, requestingUsername);
        }
        
        private async Task<MemberDto> GetSelfInfo(string username)
        {
            var member = await _unitOfWork.UserRepository.GetMember(username, true).FirstOrDefaultAsync();
            if(member == null) throw new NotFoundException("User not found");
            return member;
        }

        private async Task<MemberDto> GetRequestedUserInfo(string username, string requestingUsername)
        {
            var invertedRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(username, requestingUsername);
            if (invertedRelation != null && invertedRelation.Relation == RelationStatus.BLOCKED)
                throw new NotFoundException("User not found");

            var userQuery = _unitOfWork.UserRepository.GetMember(username, false);
            var userRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(requestingUsername, username);

            if (userRelation != null && userRelation.Relation == RelationStatus.BLOCKED)
                userQuery = userQuery.Select(MemberDto.BlockedUserSelector);
            else
                userQuery = userQuery.Select(MemberDto.NonLogedUserSelector);

            var user = userQuery.FirstOrDefault();
            if (user == null) throw new NotFoundException("User not found");

            if (userRelation != null)
                user.RelationTo = userRelation.Relation.ToString();

            return user;
        }


        public async Task<PagedList<MemberDto>> GetUsers(UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsername(userParams.CurrentUsername);

            var users = _unitOfWork.UserRepository.GetMembers(userParams);

            var blockedIDs = await _userRelationService.GetBlockedRelationsIds(user.Id);

            users = users
                .Select(MemberDto.NonLogedUserSelector)
                .Where(u => !blockedIDs.Contains(u.Id));

            return await PagedList<MemberDto>.CreateAsync(users.AsNoTracking(),
                userParams.PageNumber, userParams.PageSize);
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
            var user = await _unitOfWork.UserRepository.GetUserByUsername(username);

            _mapper.Map(memberUpdateDto, user);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidActionException(FormatErrorMessage(result));
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
            var user = await _unitOfWork.UserRepository.GetUserById(userId);
            if (user == null)
                throw new UnauthorizedException();

            if (passwordChangeDto.Password != passwordChangeDto.RepeatPassword)
                throw new InvalidActionException("New password and repeat password must be the same");
            if (passwordChangeDto.Password == passwordChangeDto.OldPassword)
                throw new InvalidActionException("New password can't be same as current");

            var changeResult = await _userManager.ChangePasswordAsync(user, passwordChangeDto.OldPassword, passwordChangeDto.Password);
            if (changeResult.Errors.Any())
                throw new InvalidActionException(FormatErrorMessage(changeResult));
        }
        
        public async Task DeactivateProfile(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(id, true);

            _unitOfWork.UserRepository.Remove(user);
            if (await _unitOfWork.Complete())
            {
                await _cloudinaryService.DeletePhotos(user.Photos.Select(p => p.PublicId));
                return;
            }

            throw new ServerErrorException("Failed to delete user: " + user.UserName);
        }

        public async Task<AppUser> Register(RegisterDto registerDto)
        {
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            user.PublicActivity = true;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new InvalidActionException(FormatErrorMessage(result));

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
                throw new InvalidActionException(FormatErrorMessage(result));
        
            return user;
        }

        public async Task<AppUser> Login(LoginDto loginDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameIncludeRefreshTokens(loginDto.Username);

            if (user == null)
                throw new UnauthorizedException("Invalid username or password!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (result.IsLockedOut)
                throw new UnauthorizedException("Maximum failed login attempts reached! Try again in " + user.LockoutEnd.Value.DateTime.GetTimeDifferenceInMinutes() + " min");
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid username or password!");

            return user;
        }

        public async Task<AppUser> LoginGoogle(ExternalAuthDto externalAuthDto, GoogleJsonWebSignature.Payload payload)
        {
            var loginInfo = new UserLoginInfo(externalAuthDto.Provider, payload.Subject, externalAuthDto.Provider);
            var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user == null)
            {
                user = await _unitOfWork.UserRepository.GetUserByEmail(payload.Email);

                if (user == null)
                {
                    user = await RegisterGoogle(payload, loginInfo);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, loginInfo);
                }

                if (user == null)
                    throw new InvalidActionException("Invalid External Authentication.");
            } else
            {
                user = await _unitOfWork.UserRepository.GetUserByEmail(payload.Email);
            }

            if (user.Photos == null)
                user.Photos = new List<Photo>();
            return user;
        }

        private async Task<AppUser> RegisterGoogle(GoogleJsonWebSignature.Payload payload, UserLoginInfo loginInfo)
        {
            AppUser user = new()
            {
                UserName = payload.Email,
                Email = payload.Email,
                KnownAs = payload.GivenName,
                PublicActivity = true
            };
            await _userManager.CreateAsync(user);

            await _userManager.AddToRoleAsync(user, "Member");
            await _userManager.AddLoginAsync(user, loginInfo);
            return user;
        }

        private static string FormatErrorMessage(IdentityResult result) => string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
    }
}
