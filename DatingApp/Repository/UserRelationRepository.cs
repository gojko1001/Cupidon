using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Repository.Interfaces;
using DatingApp.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class UserRelationRepository : IUserRelationRepository
    {
        private readonly DataContext _context;

        public UserRelationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserRelation> GetUserRelation(int sourceUserId, int likedUserId)
        {
            return await _context.UserRelations.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<PagedList<RelationDto>> GetUserRelations(RelationParams relationParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var relations = _context.UserRelations.AsQueryable();

            if (relationParams.Predicate == "liked")
            {
                relations = relations.Where(like => like.SourceUserId == relationParams.UserId && like.Relation == RelationStatus.LIKED);
                users = relations.Select(like => like.RelatedUser);
            }

            if(relationParams.Predicate == "likedBy")
            {
                relations = relations.Where(like => like.RelatedUserId == relationParams.UserId && like.Relation == RelationStatus.LIKED);
                users = relations.Select(like => like.SourceUser);
            }

            if (relationParams.Predicate == "blocked")
            {
                relations = relations.Where(like => like.SourceUserId == relationParams.UserId && like.Relation == RelationStatus.BLOCKED);
                users = relations.Select(like => like.SourceUser);
            }
            // TODO: Cover Case when predicate is none of the above
            var likedUsers = users.Select(user => new RelationDto
            {
                Id = user.Id,
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.GetAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City
            });

            return await PagedList<RelationDto>.CreateAsync(likedUsers, relationParams.PageNumber, relationParams.PageSize);
        }

        public async Task<AppUser> GetUserWithRelations(string username)
        {
            return await _context.Users
                .Include(u => u.RelationToUsers)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}
