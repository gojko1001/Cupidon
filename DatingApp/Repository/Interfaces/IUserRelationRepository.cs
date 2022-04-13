﻿using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;

namespace DatingApp.Repository.Interfaces
{
    public interface IUserRelationRepository
    {
        Task<UserRelation> GetUserRelation(int sourceUserId, int relatedUserId);
        Task<UserRelation> GetUserRelation(string sourceUsername, string relatedUsername);
        Task<AppUser> GetUserWithRelations(string username);
        Task<PagedList<RelationDto>> GetUserRelations(RelationParams relationParams);
    }
}
