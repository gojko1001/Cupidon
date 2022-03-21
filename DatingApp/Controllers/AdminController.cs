using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DatingApp.DTOs;
using DatingApp.Services.interfaces;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;

        public AdminController(IUserService userService, IPhotoService photoService)
        {
            _userService = userService;
            _photoService = photoService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRole()
        {
           return Ok(await _userService.GetUsersWithRole());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForApproval()
        {
            return Ok(await _photoService.GetUnapprovedPhotos());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPut("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            await _photoService.ApprovePhoto(photoId);
            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            await _photoService.RejectPhoto(photoId);
            return Ok();
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
        {
            var selectedRoles = roles.Split(',').ToArray();

            return Ok(await _userService.EditRoles(username, selectedRoles));
        }
    }
}
