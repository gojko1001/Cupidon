using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;
        private Mock<UserManager<AppUser>> _userManager;
        private Mock<SignInManager<AppUser>> _signInManager;
        private UserService _userService;

        private AppUser _testUser = new AppUser { Id = 1, UserName = "alice"};

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _userManager = MockUserManager<AppUser>();
            _signInManager = MockSignInManager<AppUser>();

            _userService = new UserService(_unitOfWork.Object, _mapper.Object, _userManager.Object, _signInManager.Object);
        }

        [Test]
        public async Task UpdateUser_WhenCalled_SaveChangesToDB()
        {
            _unitOfWork.Setup(u => u.UserRepository.GetUserByUsername(_testUser.UserName)).ReturnsAsync(_testUser);
            _unitOfWork.Setup(u => u.Complete()).ReturnsAsync(true);

            await _userService.UpdateUser(new MemberUpdateDto(), _testUser.UserName);

            _unitOfWork.Verify(u => u.UserRepository.Update(It.IsAny<AppUser>()));
        }


        [Test]
        public void EditRoles_UserDoesntExist_ThrowInvalidActionException()
        {
            _userManager.Setup(mgr => mgr.FindByNameAsync(It.IsAny<string>())).Verifiable();

            Assert.That(async () => await _userService.EditRoles("nonexistingUser", new string[] {"role1"}), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public async Task EditRoles_WhenCalled_ReturnListOfEditedRoles()
        {
            var roles = new string[] { "role1", "role2" };
            IList<string> editedRoles = new List<string>(roles);
            _userManager.Setup(mgr => mgr.FindByNameAsync(_testUser.UserName)).ReturnsAsync(_testUser);
            _userManager.Setup(mgr => mgr.AddToRolesAsync(_testUser, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(mgr => mgr.RemoveFromRolesAsync(_testUser, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(mgr => mgr.GetRolesAsync(_testUser)).ReturnsAsync(editedRoles);

            var result = await _userService.EditRoles(_testUser.UserName, roles);

            Assert.That(result, Is.EqualTo(roles));
        }


        [Test]
        public void ChangePassword_UserDoesntExist_ThrowUnauthorizedException()
        {
            _unitOfWork.Setup(u => u.UserRepository.GetUserById(It.IsAny<int>(), It.IsAny<bool>())).Verifiable();

            Assert.That(async () => await _userService.ChangePassword(new PasswordChangeDto(), 1), Throws.Exception.TypeOf<UnauthorizedException>());
        }

        [Test]
        public void ChangePassword_PasswordAndRepeatPasswordNotMatch_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(u => u.UserRepository.GetUserById(_testUser.Id, It.IsAny<bool>())).ReturnsAsync(_testUser);

            Assert.That(async () => await _userService.ChangePassword(
                new PasswordChangeDto { OldPassword = "a", Password = "b", RepeatPassword = "c" }, 1), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public void ChangePassword_PasswordAndOldPasswordAreSame_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(u => u.UserRepository.GetUserById(_testUser.Id, It.IsAny<bool>())).ReturnsAsync(_testUser);

            Assert.That(async () => await _userService.ChangePassword(
                new PasswordChangeDto { OldPassword = "a", Password = "a", RepeatPassword = "a" }, 1), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public async Task ChangePassword_WithValidParameters_InvokeChangePasswordOfUserManager()
        {
            _unitOfWork.Setup(u => u.UserRepository.GetUserById(_testUser.Id, It.IsAny<bool>())).ReturnsAsync(_testUser);
            _userManager.Setup(mgr => mgr.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            await _userService.ChangePassword(new PasswordChangeDto { OldPassword = "a", Password = "b", RepeatPassword = "b" }, 1);

            _userManager.Verify(mgr => mgr.ChangePasswordAsync(_testUser, "a", "b"));
        }


        [Test]
        public void Register_UserAlreadyExists_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(u => u.UserRepository.UserExists(_testUser.UserName)).ReturnsAsync(true);

            Assert.That(async () => await _userService.Register(
                new RegisterDto() { Username = _testUser.UserName}), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public async Task Register_WhenCalled_ReturnRegisteredUserAsync()
        {
            var registerDto = new RegisterDto
            {
                Username = _testUser.UserName,
                Password = "Password"
            };
            var user = new AppUser
            {
                UserName = registerDto.Username,
            };
            _unitOfWork.Setup(u => u.UserRepository.UserExists(_testUser.UserName)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<AppUser>(registerDto)).Returns(user);
            _userManager.Setup(mgr => mgr.CreateAsync(user, registerDto.Password)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(mgr => mgr.AddToRoleAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _userService.Register(registerDto);

            Assert.That(result.UserName, Is.EqualTo(registerDto.Username).IgnoreCase);
        }


        [Test]
        public void Login_UserDoesntExist_ThrowUnauthorizedException()
        {
            var loginDto = new LoginDto
            {
                Username = "nonexistinguser",
                Password = "Password"
            };
            _unitOfWork.Setup(u => u.UserRepository.GetUserByUsernameIncludeRefreshTokens(It.IsAny<string>())).Verifiable();

            Assert.That(async () => await _userService.Login(loginDto), Throws.Exception.TypeOf<UnauthorizedException>());
        }

        [Test]
        public void Login_IncorrectPassword_ThrowUnauthorizedException()
        {
            var loginDto = new LoginDto
            {
                Username = _testUser.UserName,
                Password = "Incorrect password"
            };
            _unitOfWork.Setup(u => u.UserRepository.GetUserByUsernameIncludeRefreshTokens(_testUser.UserName)).ReturnsAsync(_testUser);
            _signInManager.Setup(s => s.CheckPasswordSignInAsync(_testUser, loginDto.Password, It.IsAny<bool>())).ReturnsAsync(SignInResult.Failed);

            Assert.That(async () => await _userService.Login(loginDto), Throws.Exception.TypeOf<UnauthorizedException>());
        }

        [Test]
        public async Task Login_UserExistsAndPasswordValid_ReturnLoggedInUserAsync()
        {
            var loginDto = new LoginDto
            {
                Username = _testUser.UserName,
                Password = "Password"
            };
            _unitOfWork.Setup(u => u.UserRepository.GetUserByUsernameIncludeRefreshTokens(_testUser.UserName)).ReturnsAsync(_testUser);
            _signInManager.Setup(s => s.CheckPasswordSignInAsync(_testUser, loginDto.Password, It.IsAny<bool>())).ReturnsAsync(SignInResult.Success);

            var result = await _userService.Login(loginDto);

            Assert.That(result.UserName, Is.EqualTo(loginDto.Username));
        }


        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            return mgr;
        }

        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            var userManager = MockUserManager<TUser>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrinicipalFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();

            return new Mock<SignInManager<TUser>>(userManager.Object, contextAccessor.Object, userPrinicipalFactory.Object, null, null, null, null);
        }
    }
}
