namespace DatingApp.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IPhotoRepository PhotoRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }
        IGroupRepository GroupRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
