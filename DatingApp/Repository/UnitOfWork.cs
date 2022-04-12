using AutoMapper;
using DatingApp.Data;
using DatingApp.Repository.Interfaces;

namespace DatingApp.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IPhotoRepository PhotoRepository => new PhotoRepository(_context);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public IUserRelationRepository UserRelationRepository => new UserRelationRepository(_context);

        public IGroupRepository GroupRepository => new GroupRepository(_context);

        public IRefreshTokenRepository RefreshTokenRepository => new RefreshTokenRepository(_context);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
