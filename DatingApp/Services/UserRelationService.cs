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
                throw new NotFoundException("User not found");

            var invertedRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(likedUser.Id, sourceUser.Id);
            if (invertedRelation != null && invertedRelation.Relation == RelationStatus.BLOCKED)
                throw new NotFoundException("User not found");

            var userRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(sourceUser.Id, likedUser.Id);
            if (userRelation != null)
            {
                if (userRelation.Relation == RelationStatus.LIKED)
                    throw new InvalidActionException("You already liked this user!");
                throw new InvalidActionException("You blocked this user!");
            }


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

        public async Task AddBlock(string sourceUsername, string blockedUsername)
        {
            var sourceUser = await _unitOfWork.UserRelationRepository.GetUserWithRelations(sourceUsername);
            var blockedUser = await _unitOfWork.UserRepository.GetUserByUsername(blockedUsername);

            if (blockedUser == null)
                throw new NotFoundException("User not found");

            var invertedRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(blockedUser.Id, sourceUser.Id);
            if (invertedRelation != null)
            {
                if(invertedRelation.Relation == RelationStatus.BLOCKED)
                    throw new NotFoundException("User not found");
                var blockedUserWithRelations = await _unitOfWork.UserRelationRepository.GetUserWithRelations(blockedUsername);
                blockedUserWithRelations.RelationToUsers.Remove(invertedRelation);
            }

            var userRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(sourceUser.Id, blockedUser.Id);
            if (userRelation != null)
            {
                if (userRelation.Relation == RelationStatus.BLOCKED)
                    throw new InvalidActionException("You already blocked this user!");
                userRelation.Relation = RelationStatus.BLOCKED;
            }
            else
            {
                userRelation = new UserRelation
                {
                    SourceUserId = sourceUser.Id,
                    RelatedUserId = blockedUser.Id,
                    Relation = RelationStatus.BLOCKED
                };

                sourceUser.RelationToUsers.Add(userRelation);
            }

            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to block user!");
        }

        public async Task RemoveRelation(string sourceUsername, string relatedUsername)
        {
            var sourceUser = await _unitOfWork.UserRelationRepository.GetUserWithRelations(sourceUsername);
            var relatedUser = await _unitOfWork.UserRepository.GetUserByUsername(relatedUsername);

            if (relatedUser == null)
                throw new NotFoundException("User not found");

            var invertedRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(relatedUser.Id, sourceUser.Id);
            if (invertedRelation != null && invertedRelation.Relation == RelationStatus.BLOCKED)
                throw new NotFoundException("User not found");

            var userRelation = await _unitOfWork.UserRelationRepository.GetUserRelation(sourceUser.Id, relatedUser.Id);
            if(userRelation == null)
                throw new InvalidActionException("No relation to selected user");

            sourceUser.RelationToUsers.Remove(userRelation);

            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to remove relation!");
        }

        public async Task<PagedList<RelationDto>> GetUserRelations(RelationParams relationParams)
        {
            return await _unitOfWork.UserRelationRepository.GetUserRelations(relationParams);
        }
    }
}
