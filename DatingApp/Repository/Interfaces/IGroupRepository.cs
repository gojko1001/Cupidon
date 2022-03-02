using DatingApp.Entities;

namespace DatingApp.Repository.Interfaces
{
    public interface IGroupRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnectionAsync(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        Task<bool> SaveAllAsync();
    }
}
