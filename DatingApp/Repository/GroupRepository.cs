using DatingApp.Data;
using DatingApp.Entities;
using DatingApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly DataContext _context;

        public GroupRepository(DataContext context)
        {
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public async Task<Connection> GetConnectionAsync(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(g => g.Connections)
                .FirstOrDefaultAsync(g => g.Name == groupName);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
