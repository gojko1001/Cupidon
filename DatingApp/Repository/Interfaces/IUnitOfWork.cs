namespace DatingApp.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        IMessageRepository MessageRepository { get; }
        IUserRelationRepository UserRelationRepository { get; }
        IGroupRepository GroupRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
