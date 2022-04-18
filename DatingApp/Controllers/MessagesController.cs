using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Services.interfaces;
using DatingApp.Utils.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUsername();
            messageParams.UserId = User.GetId();
            var messages = await _messageService.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveMessage(int id)
        {
            var username = User.GetUsername();
            await _messageService.RemoveMessage(id, username);
            return Ok();
        }

    }
}
