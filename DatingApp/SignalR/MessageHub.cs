using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public MessageHub(IMessageRepository messageRepository, IGroupRepository groupRepository, IMapper mapper, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var caller = Context.User.GetUsername();
            var otherUser = Context.GetHttpContext().Request.Query["user"].ToString();
            var groupName = GetGroupName(caller, otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            var messages = await _messageRepository.GetMessageThread(caller, otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            if (createMessageDto.Content == null)
                throw new HubException("Message is empty");

            var sender = await _userRepository.GetByIdAsync(Context.User.GetId());

            if (sender.UserName == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("Can't send a message to yourself");

            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
                throw new HubException("Not Found user");

            var message = new Message
            {
                SenderId = sender.Id,
                SenderUsername = sender.UserName,
                RecipientId = recipient.Id,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                DateSent = DateTime.Now,
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _groupRepository.GetMessageGroup(groupName);
            if(group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }


        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await _groupRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                _groupRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await _groupRepository.SaveAllAsync();
        }

        private async Task RemoveFromMessageGroup()
        {
            var connection = await _groupRepository.GetConnectionAsync(Context.ConnectionId);
            _groupRepository.RemoveConnection(connection);
            await _groupRepository.SaveAllAsync();
        }

        private static string GetGroupName(string caller, string other)
        {
            var stringCompare = string.Compare(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
           
        }
    }
}
