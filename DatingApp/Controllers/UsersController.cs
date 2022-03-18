using AutoMapper;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            userParams.CurrentUsername = User.GetUsername();
            var users = await _userService.GetUsers(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userService.GetUser(username, User.GetUsername() == username);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var photo = await _userService.AddPhoto(file, User.GetId());
            if(photo != null)
            {
                return CreatedAtRoute("GetUser", new { Username = User.GetUsername() }, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Error while adding new photo!");
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            await _userService.UpdateUser(memberUpdateDto, User.GetUsername());
            return NoContent();
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            await _userService.SetMainPhoto(photoId, User.GetId());
            return NoContent();
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> RemovePhoto(int photoId)
        {
            await _userService.RemovePhoto(photoId);
            return Ok();
        }
    }
}
