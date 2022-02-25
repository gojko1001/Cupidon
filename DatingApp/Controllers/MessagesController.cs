using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using DatingApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUsername();
            var messages = await _messageRepository.GetMessagesForUserAsync(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            return Ok(await _messageRepository.GetMessageThread(User.GetUsername(), username));
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            if (createMessageDto.Content == null)
                return BadRequest("Message is empty");

            var sender = await _userRepository.GetByIdAsync(User.GetId());

            if (sender.UserName == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("Can't send a message to yourself");

            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient == null)
                return NotFound();

            var message = new Message
            {
                SenderId = sender.Id,
                SenderUsername = sender.UserName,
                RecipientId = recipient.Id,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                DateSent = DateTime.Now,
            };
            _messageRepository.AddMessage(message);
            if(await _messageRepository.SaveAllAsync())
                return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send a message");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {

            var username = User.GetUsername();
            var message = await _messageRepository.GetMessageAsync(id);
            if (message == null)
                return BadRequest("Message doesn't exists");
            if(message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();
            if (message.Sender.UserName == username)
                message.SenderDeleted = true;
            if(message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
                _messageRepository.DeleteMessage(message);
            if (await _messageRepository.SaveAllAsync())
                return Ok();
            return BadRequest("Failed to delete message");
        }

    }
}
