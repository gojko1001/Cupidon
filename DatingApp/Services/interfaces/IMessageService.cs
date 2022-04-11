using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services.interfaces
{
    public interface IMessageService
    {
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(string caller, string otherUser);
        Task<Message> SendMessage(CreateMessageDto createMessageDto);
        Task<Message> RemoveMessage(int messageId, string username);
    }
}
