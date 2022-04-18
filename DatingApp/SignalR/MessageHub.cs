using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public MessageHub(IUnitOfWork unitOfWork, IMessageService messageService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _messageService = messageService;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var caller = Context.User.GetUsername();
            var otherUser = Context.GetHttpContext().Request.Query["user"].ToString();
            var groupName = GetGroupName(caller, otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageService.GetMessageThread(caller, otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", _mapper.Map<IEnumerable<MessageDto>>(messages));
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            if (createMessageDto.Content == null)
                throw new HubException("Message is empty");

            var senderUsername = Context.User.GetUsername();
            if (senderUsername == createMessageDto.RecipientUsername)
                throw new HubException("Can't send a message to yourself");

            createMessageDto.SenderUsername = senderUsername;

            var message = await _messageService.SendMessage(createMessageDto);
            if(message != null)
                await Clients.Group(GetGroupName(senderUsername, createMessageDto.RecipientUsername))
                    .SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _unitOfWork.GroupRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                _unitOfWork.GroupRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if(await _unitOfWork.Complete())
                return group;
            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var connection = await _unitOfWork.GroupRepository.GetConnectionAsync(Context.ConnectionId);
            var group = await _unitOfWork.GroupRepository.GetGroupForConnection(connection.ConnectionId);
            _unitOfWork.GroupRepository.RemoveConnection(connection);
            if (await _unitOfWork.Complete())
                return group;
            throw new HubException("Failed to remove from group");
        }


        private static string GetGroupName(string caller, string other)
        {
            var stringCompare = string.Compare(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
