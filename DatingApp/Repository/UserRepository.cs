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

        public async Task<IEnumerable<AppUser>> GetAll()
            => await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();


        public async Task<MemberDto> GetMember(string username, bool isCurrentUser)
        {
            var query = _context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsQueryable();
            if(isCurrentUser)
                query = query.IgnoreQueryFilters();
            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<AppUser> GetMembers(UserParams userParams)
        {
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            
            var query = _context.Users.AsQueryable()
                .Where(u => u.UserName != userParams.CurrentUsername)
                .Where(u => u.Gender == userParams.Gender)
                .Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return query;
        }

        public async Task<AppUser> GetUserById(int id, bool isCurrentUser)
        {
            var query = _context.Users
                       .Include(p => p.Photos)
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
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(u => u.UserName == username);

        public Task<AppUser> GetUserByUsernameIncludeRefreshTokens(string username)
        {
            return _context.Users
                .Include(p => p.Photos)
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.UserName == username.ToLower());
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<bool> UserExists(string username) => await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
