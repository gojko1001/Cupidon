using DatingApp.DTOs;
using DatingApp.Extensions;
using DatingApp.Services.interfaces;
using DatingApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetId();
            PagedList<LikeDto> users = await _likeService.GetUserLikes(likesParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }


        [HttpPost("{username}")]
        public async Task<ActionResult> AddLikeAsync(string username)
        {
            var sourceUsername = User.GetUsername();
            if (sourceUsername == username)
                return BadRequest("You cannot like yourself");
            await _likeService.AddLike(sourceUsername, username);
            return Ok();
        }
    }
}
