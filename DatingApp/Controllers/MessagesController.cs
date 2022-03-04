using DatingApp.DTOs;
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
        private readonly IUnitOfWork _unitOfWork;

        public MessagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.UserName = User.GetUsername();
            var messages = await _unitOfWork.MessageRepository.GetMessagesForUserAsync(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {

            var username = User.GetUsername();
            var message = await _unitOfWork.MessageRepository.GetMessageAsync(id);
            if (message == null)
                return BadRequest("Message doesn't exists");
            if(message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();
            if (message.Sender.UserName == username)
                message.SenderDeleted = true;
            if(message.Recipient.UserName == username)
                message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
                _unitOfWork.MessageRepository.DeleteMessage(message);
            if (await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to delete message");
        }

    }
}
