using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Repository.Interfaces;
using DatingApp.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public IQueryable<MemberDto> GetMember(string username, bool isCurrentUser)
        {
            var query = _context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
            if (isCurrentUser)
                query = query.IgnoreQueryFilters();
            return query;
        }

        public IQueryable<MemberDto> GetMembers(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userParams.SearchString))
                query = GetUsersBySearchString(userParams.SearchString);

            if (!string.IsNullOrEmpty(userParams.Gender))
                query = query.Where(u => u.Gender == userParams.Gender);
            
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.UserName != userParams.CurrentUsername)
                         .Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                "lastActive" => query.OrderByDescending(u => u.LastActive),
                _ => query
            };

            return query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
        }

        private IQueryable<AppUser> GetUsersBySearchString(string searchString)
        {
            var query = _context.Users.AsQueryable();
            var singleWordQuery = query.Take(0);

            var searchStrings = searchString.ToLower().Split(' ').ToList();
            foreach (var str in searchStrings)
            {
                query = query.Where(u => u.UserName.ToLower().Contains(str) || u.KnownAs.ToLower().Contains(str));

                singleWordQuery = singleWordQuery.Union(_context.Users.Where(u => u.UserName.ToLower().Contains(str) || u.KnownAs.ToLower().Contains(str)));
            }
            return query.Union(singleWordQuery);
        }

        public async Task<AppUser> GetUserById(int id, bool isCurrentUser)
        {
            var query = _context.Users
                       .Include(u => u.Photos)
                       .AsQueryable();
            if(isCurrentUser)
                query = query.IgnoreQueryFilters();
            return await query
                       .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId) 
            => await _context.Users
                .Include(u => u.Photos)
                .Where(u => u.Photos.Any(p => p.Id == photoId))
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync();

        public async Task<AppUser> GetUserByUsername(string username) 
            => await _context.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => u.UserName == username);

        public Task<AppUser> GetUserByUsernameIncludeRefreshTokens(string username)
        {
            return _context.Users
                .Include(u => u.Photos)
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.UserName == username.ToLower());
        }

        public Task<AppUser> GetUserByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Photos)
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.Email == email);
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
