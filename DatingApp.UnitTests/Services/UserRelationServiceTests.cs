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
            _unitOfWork.Setup(uow => uow.UserRepository.GetUserByUsername(RELATED_USER)).ReturnsAsync(new AppUser
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
        public void AddLike_LikeUserThatBlockedSourceUser_ThrowNotFoundException()
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
        [TestCase(RelationStatus.LIKED)]
        [TestCase(RelationStatus.BLOCKED)]
        public void AddLike_LikeAlreadyRelatedUser_ThrowInvalidActionException(RelationStatus status)
        {
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(1, 2)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 1,
                SourceUserId = 2,
                Relation = status
            });

            Assert.That(async () => await _relationService.AddLike(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<InvalidActionException>());
        }


        [Test]
        public async Task AddLike_LikeOtherUser_AddLikeRelationToDatabase()
        {
            _unitOfWork.Setup(uow => uow.Complete()).ReturnsAsync(true);

            await _relationService.AddLike(SOURCE_USER, RELATED_USER);

            _unitOfWork.Verify(uow => uow.Complete());
        }



        [Test]
        public void AddBlock_BlockNonExistingUser_ThrowNotFoundException()
        {
            Assert.That(async () => await _relationService.AddBlock(SOURCE_USER, NON_EXISTING_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public void AddBlock_BlockUserThatBlockedSourceUser_ThrowNotFoundException()
        {
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(2, 1)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 2,
                SourceUserId = 1,
                Relation = RelationStatus.BLOCKED
            });

            Assert.That(async () => await _relationService.AddBlock(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public void AddBlock_BlockAlreadyBlockedUser_ThrowInvalidActionException()
        {
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(1, 2)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 1,
                SourceUserId = 2,
                Relation = RelationStatus.BLOCKED
            });

            Assert.That(async () => await _relationService.AddBlock(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<InvalidActionException>());
        }

        [Test]
        public async Task AddBlock_BlockOtherUser_AddUserRelationToDatabase()
        {
            _unitOfWork.Setup(uow => uow.Complete()).ReturnsAsync(true);

            await _relationService.AddBlock(SOURCE_USER, RELATED_USER);

            _unitOfWork.Verify(uow => uow.Complete());
        }



        [Test]
        public void RemoveRelation_BlockNonExistingUser_ThrowNotFoundException()
        {
            Assert.That(async () => await _relationService.RemoveRelation(SOURCE_USER, NON_EXISTING_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public void RemoveRelation_BlockedBySourceUser_ThrowNotFoundException()
        {
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(2, 1)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 2,
                SourceUserId = 1,
                Relation = RelationStatus.BLOCKED
            });

            Assert.That(async () => await _relationService.RemoveRelation(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<NotFoundException>());
        }

        [Test]
        public void RemoveRelation_RelationNotExists_ThrowInvalidActionException()
        {
            Assert.That(async () => await _relationService.AddBlock(SOURCE_USER, RELATED_USER), Throws.Exception.TypeOf<InvalidActionException>());
        }


        [Test]
        public async Task RemoveRelation_RemoveRelationToOtherUser_RemoveUserRelationFromDatabase()
        {
            _unitOfWork.Setup(uow => uow.Complete()).ReturnsAsync(true);
            _unitOfWork.Setup(uow => uow.UserRelationRepository.GetUserRelation(1, 2)).ReturnsAsync(new UserRelation
            {
                RelatedUserId = 1,
                SourceUserId = 2,
            });

            await _relationService.RemoveRelation(SOURCE_USER, RELATED_USER);

            _unitOfWork.Verify(uow => uow.Complete());
        }



        [Test]
        public async Task GetUserRelations_WhenCalled_InvokeGetUserRelationsFromDatabase()
        {
            var likeParameters = new RelationParams();

            await _relationService.GetUserRelations(likeParameters);

            _unitOfWork.Verify(uow => uow.UserRelationRepository.GetUserRelations(likeParameters));
        }


        [Test]
        public async Task GetBlockedRelationsIds_WhenCalled_FormListOfIdsAsync()
        {
            var userId = 1;
            _unitOfWork.Setup(u => u.UserRelationRepository.GetBlockedRelations(userId)).ReturnsAsync(new List<UserRelation>
            {
                new UserRelation{ SourceUserId = userId, RelatedUserId = 2 },
                new UserRelation{ SourceUserId = 3, RelatedUserId = userId },
            });

            var result = await _relationService.GetBlockedRelationsIds(userId);

            Assert.That(result, Is.EquivalentTo(new List<int> { 2, 3 }));
        }
    }
}