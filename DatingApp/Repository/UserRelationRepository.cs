using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
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

        public async Task<UserRelation> GetUserRelation(int sourceUserId, int relatedUserId)
        {
            return await _context.UserRelations.FindAsync(sourceUserId, relatedUserId);
        }

        public async Task<UserRelation> GetUserRelation(string sourceUsername, string relatedUsername)
        {
            return await _context.UserRelations
                .FirstOrDefaultAsync(r => r.SourceUser.UserName == sourceUsername && r.RelatedUser.UserName == relatedUsername);
        }

        public async Task<IEnumerable<UserRelation>> GetBlockedRelations(int userId)
        {
            return await _context.UserRelations
                .Where(r => r.Relation == RelationStatus.BLOCKED && (r.SourceUserId == userId || r.RelatedUserId == userId))
                .ToListAsync();
        }

        public async Task<PagedList<RelationDto>> GetUserRelations(RelationParams relationParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var relations = _context.UserRelations.AsQueryable();

            switch (relationParams.Predicate)
            {
                case "liked":
                    relations = relations.Where(r => r.SourceUserId == relationParams.UserId && r.Relation == RelationStatus.LIKED);
                    users = relations.Select(r => r.RelatedUser);
                    break;
                case "likedBy":
                    relations = relations.Where(r => r.RelatedUserId == relationParams.UserId && r.Relation == RelationStatus.LIKED);
                    users = relations.Select(r => r.SourceUser);
                    break;
                case "blocked":
                    relations = relations.Where(r => r.SourceUserId == relationParams.UserId && r.Relation == RelationStatus.BLOCKED);
                    users = relations.Select(r => r.RelatedUser);
                    break;
                case "blockedBy":
                    relations = relations.Where(r => r.RelatedUserId == relationParams.UserId && r.Relation == RelationStatus.BLOCKED);
                    users = relations.Select(r => r.SourceUser);
                    break;
                default:
                    throw new InvalidActionException("Bad predicate");
            }

            var relatedUsers = users.Select(user => new RelationDto
            {
                Id = user.Id,
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.GetAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City
            });

            return await PagedList<RelationDto>.CreateAsync(relatedUsers, relationParams.PageNumber, relationParams.PageSize);
        }

        public async Task<AppUser> GetUserWithRelations(string username)
        {
            return await _context.Users
                .Include(u => u.RelationToUsers)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

    }
}
