using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.SignalR;
using DatingApp.Utils.Pagination;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper,
            IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _tracker = tracker;
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            return await _unitOfWork.MessageRepository.GetMessagesForUserAsync(messageParams);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string caller, string otherUser)
        {
            var messages = await _unitOfWork.MessageRepository.GetMessageThread(caller, otherUser);

            var unreadMessages = messages.Where(m => m.DateRead == null &&
                                                m.RecipientUsername == caller).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }
            if (_unitOfWork.HasChanges())
                await _unitOfWork.Complete();    
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<Message> SendMessage(CreateMessageDto createMessageDto)
        {
            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.SenderUsername);
            var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
                throw new NotFoundException("Recepient not found");

            var message = new Message
            {
                SenderId = sender.Id,
                SenderUsername = sender.UserName,
                RecipientId = recipient.Id,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _unitOfWork.GroupRepository.GetMessageGroup(groupName);
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new
                    {
                        username = sender.UserName,
                        knownAs = sender.KnownAs,
                        content = message.Content
                    });
                }
            }

            _unitOfWork.MessageRepository.AddMessage(message);
            await _unitOfWork.Complete();
            return message;
        }

        public async Task RemoveMessage(int messageId, string username)
        {
            var message = await _unitOfWork.MessageRepository.GetMessageAsync(messageId);
            if (message == null)
                throw new InvalidActionException("Message doesn't exists");
            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                throw new UnauthorizedException();
            if (message.Sender.UserName == username)
                message.SenderDeleted = true;
            if (message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                _unitOfWork.MessageRepository.RemoveMessage(message);
            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to delete message");
        }


        private static string GetGroupName(string caller, string other)
        {
            var stringCompare = string.Compare(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
