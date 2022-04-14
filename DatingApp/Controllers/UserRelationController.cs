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
    public class UserRelationController : ControllerBase
    {
        private readonly IUserRelationService _relationService;

        public UserRelationController(IUserRelationService relationService)
        {
            _relationService = relationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RelationDto>>> GetUserLikes([FromQuery]RelationParams relationParams)
        {
            relationParams.UserId = User.GetId();
            PagedList<RelationDto> users = await _relationService.GetUserRelations(relationParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }


        [HttpPost("like/{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUsername = User.GetUsername();
            if (sourceUsername == username)
                return BadRequest("You cannot like yourself");
            await _relationService.AddLike(sourceUsername, username);
            return Ok();
        }

        [HttpPost("block/{username}")]
        public async Task<ActionResult> AddBlock(string username)
        {
            var sourceUsername = User.GetUsername();
            if (sourceUsername == username)
                return BadRequest("You cannot block yourself");
            await _relationService.AddBlock(sourceUsername, username);
            return Ok();
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult> RemoveRelation(string username)
        {
            var sourceUsername = User.GetUsername();
            if (sourceUsername == username)
                return BadRequest("You cannot have relation to yourself");
            await _relationService.RemoveRelation(sourceUsername, username);
            return Ok();
        }
    }
}
