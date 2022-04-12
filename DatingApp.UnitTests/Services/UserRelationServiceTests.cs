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
    public class UserRelationServiceTests
    {
        private UserRelationService _relationService;
        private Mock<IUnitOfWork> _unitOfWork;

        private const string SOURCE_USER = "alice";
        private const string RELATED_USER = "bob";
        private const string NON_EXISTING_USER = "john";

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserWithRelations(SOURCE_USER)).ReturnsAsync(new AppUser
            {
                Id = 1,
                UserName = "alice",
                RelationToUsers = new List<UserRelation>()
            });
            _unitOfWork.Setup(uow => uow.UserRepository.GetUserByUsernameAsync(RELATED_USER)).ReturnsAsync(new AppUser
            {
                Id = 2,
                UserName = "bob",
            });
            _relationService = new UserRelationService(_unitOfWork.Object);
        }

        [Test]
        public void AddLike_LikeNonExistingUser_ThrowNotFoundException()
        {
            Assert.That(async () => await _relationService.AddLike(SOURCE_USER, NON_EXISTING_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public void AddLike_LikeAlreadyLikedUser_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(1, 2)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 1,
                SourceUserId = 2,
                Relation = RelationStatus.LIKED
            });

            Assert.That(async () => await _relationService.AddLike(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public void AddLike_LikeUserThatBlockedSourceUSer_ThrowNotFoundException()
        {
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(2, 1)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 2,
                SourceUserId = 1,
                Relation = RelationStatus.BLOCKED
            });

            Assert.That(async () => await _relationService.AddLike(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task AddLike_LikeOtherUser_AddLikeRelationToDatabaseAsync()
        {
            _unitOfWork.Setup(uow => uow.Complete()).ReturnsAsync(true);

            await _relationService.AddLike(SOURCE_USER, RELATED_USER);

            _unitOfWork.Verify(uow => uow.Complete());
        }

        [Test]
        public async Task GetUserLikes_WhenCalled_InvokeGetUserLikesFromDatabase()
        {
            var likeParameters = new RelationParams();

            await _relationService.GetUserRelations(likeParameters);

            _unitOfWork.Verify(uow => uow.UserRelationRepository.GetUserRelations(likeParameters));
        }
    }
}