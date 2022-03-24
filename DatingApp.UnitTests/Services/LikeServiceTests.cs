using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services;
using DatingApp.Utils.Pagination;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.UnitTests.Services
{
    [TestFixture]
    public class LikeServiceTests
    {
        private LikeService _likeService;
        private Mock<IUnitOfWork> _unitOfWork;

        private const string SOURCE_USER = "alice";
        private const string LIKED_USER = "bob";
        private const string NON_EXISTING_USER = "john";

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork.Setup(uow => uow.LikesRepository.GetUserWithLikes(SOURCE_USER)).Returns(Task.FromResult(new AppUser
            {
                Id = 1,
                UserName = "alice",
                LikedUsers = new List<UserLike>()
            }));
            _unitOfWork.Setup(uow => uow.UserRepository.GetUserByUsernameAsync(LIKED_USER)).Returns(Task.FromResult(new AppUser
            {
                Id = 2,
                UserName = "bob",
            }));
            _likeService = new LikeService(_unitOfWork.Object);
        }

        [Test]
        public void AddLike_LikeNonExistingUser_ThrowNotFoundException()
        {
            Assert.That(async () => await _likeService.AddLike(SOURCE_USER, NON_EXISTING_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public void AddLike_LikeAlreadyLikedUser_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(uow => uow.LikesRepository.GetUserLike(1, 2)).Returns(Task.FromResult(new UserLike
            {
                LikedUserId = 1,
                SourceUserId = 2
            }));

            Assert.That(async () => await _likeService.AddLike(SOURCE_USER, LIKED_USER), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public async Task AddLike_LikeOtherUser_AddLikeRelationToDatabaseAsync()
        {
            _unitOfWork.Setup(uow => uow.Complete()).Returns(Task.FromResult(true));

            await _likeService.AddLike(SOURCE_USER, LIKED_USER);

            _unitOfWork.Verify(uow => uow.Complete());
        }

        [Test]
        public async Task GetUserLikes_WhenCalled_InvokeGetUserLikesFromDatabase()
        {
            var likeParameters = new LikesParams();

            await _likeService.GetUserLikes(likeParameters);

            _unitOfWork.Verify(uow => uow.LikesRepository.GetUserLikes(likeParameters));
        }
    }
}