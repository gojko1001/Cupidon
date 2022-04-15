using AutoMapper;
using AutoMapper.QueryableExtensions;
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


        public async Task<MemberDto> GetUser(string username, string requestingUser)
        {
            var invertedRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(username, requestingUser);
            if(invertedRelation != null && invertedRelation.Relation == RelationStatus.BLOCKED)
                throw new NotFoundException("User not found");

            var userQuery = _unitOfWork.UserRepository.GetMember(username, username == requestingUser);
            var userRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(requestingUser, username);
            if(userRelation != null && userRelation.Relation == RelationStatus.BLOCKED)
            {
                userQuery = userQuery.Select(u => new MemberDto
                {
                    Id = u.Id,
                    Age = u.Age,
                    Username = u.Username,
                    KnownAs = u.KnownAs,
                    PhotoUrl = u.PhotoUrl,
                });
            }
            var user = userQuery.FirstOrDefault();
            if(userRelation != null)
                user.RelationTo = userRelation.Relation.ToString();

            return user;
        }

        public async Task<PagedList<MemberDto>> GetUsers(UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsername(userParams.CurrentUsername);
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = (user.Gender == "male") ? "female" : "male";

            var users = _unitOfWork.UserRepository.GetMembers(userParams);

            // TODO: UserRelationSrvice method to get blocked relations from both sides
            var blocked = await _unitOfWork.UserRelationRepository.GetUserRelations(new RelationParams { UserId = user.Id, Predicate = "blocked" });
            var blockedBy = await _unitOfWork.UserRelationRepository.GetUserRelations(new RelationParams { UserId = user.Id, Predicate = "blockedBy" });
            var blockedIDs = blocked.Select(u => u.Id).Union(blockedBy.Select(u => u.Id));
             
            users = users.Where(u => !blockedIDs.Contains(u.Id));

            return await PagedList<MemberDto>.CreateAsync(users.ProjectTo<MemberDto>(
                _mapper.ConfigurationProvider).AsNoTracking(),
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
            var user = await _unitOfWork.UserRepository.GetUserById(userId);
            if (user == null)
                throw new UnauthorizedException();

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
            if (await _unitOfWork.UserRepository.UserExists(registerDto.Username))
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
            var user = await _unitOfWork.UserRepository.GetUserByUsernameIncludeRefreshTokens(loginDto.Username);

            if (user == null)
                throw new UnauthorizedException("Invalid username or password!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid username or password!");

            return user;
        }
    }
}
