using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await GetUserFromToken();
            userParams.CurrentUsername = user.UserName;
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return base.Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: User.GetUsername() == username);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await GetUserFromToken();

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);

            if(await _unitOfWork.Complete())
            {
                return CreatedAtRoute("GetUser", new {Username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Error while adding new photo!");
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await GetUserFromToken();

            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);
            if(await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await GetUserFromToken();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain)
                return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
            
            if (currentMain != null)
                currentMain.IsMain = false;
            photo.IsMain = true;

            if(await _unitOfWork.Complete())
                return NoContent();
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await GetUserFromToken();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if(photo == null)
                return NotFound();
            if(photo.IsMain)
                return BadRequest("You cannot delete your main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null)
                    return BadRequest(result.Error);
            }
            user.Photos.Remove(photo);
            if(await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to delete photo");
        }


        private async Task<AppUser> GetUserFromToken() => await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
    }
}
