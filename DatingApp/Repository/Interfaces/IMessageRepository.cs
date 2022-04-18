using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;

namespace DatingApp.Repository.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void RemoveMessage(Message message);
        Task<Message> GetMessage(int id);
        IQueryable<MessageDto> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(string currentUsername, string recipientUsername);
    }
}
