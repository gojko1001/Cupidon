using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services
{
    public class UserRelationService : IUserRelationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRelationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddLike(string sourceUsername, string likedUsername)
        {
            var sourceUser = await _unitOfWork.UserRelationRepository.GetUserWithRelations(sourceUsername);
            var likedUser = await _unitOfWork.UserRepository.GetUserByUsername(likedUsername);

            if (likedUser == null)
                throw new NotFoundException();

            var userRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(sourceUser.Id, likedUser.Id);
            if (userRelation != null)
            {
                if (userRelation.Relation == RelationStatus.LIKED)
                    throw new InvalidActionException("You already liked this user!");
                throw new InvalidActionException("You blocked this user!");
            }

            var invertedRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(likedUser.Id, sourceUser.Id);
            if (invertedRelation != null && invertedRelation.Relation == RelationStatus.BLOCKED)
                throw new NotFoundException();

            userRelation = new UserRelation
            {
                SourceUserId = sourceUser.Id,
                RelatedUserId = likedUser.Id,
                Relation = RelationStatus.LIKED
            };

            sourceUser.RelationToUsers.Add(userRelation);

            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to like user!");
        }

        public async Task<PagedList<RelationDto>> GetUserRelations(RelationParams relationParams)
        {
            return await _unitOfWork.UserRelationRepository.GetUserRelations(relationParams);
        }
    }
}
