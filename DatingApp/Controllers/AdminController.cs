using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.DTOs;
using DatingApp.Services.interfaces;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;

        public AdminController(UserManager<AppUser> userManager, IPhotoService photoService)
        {
            _userManager = userManager;
            _photoService = photoService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRole()
        {
           return Ok(await _userManager.Users
                        .Include(r => r.Roles).ThenInclude(r => r.Role)
                        .OrderBy(u => u.UserName)
                        .Select(u => new
                        {
                            u.Id,
                            Username = u.UserName,
                            Roles = u.Roles.Select(r => r.Role.Name).ToList()
                        })
                        .ToListAsync());
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

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return BadRequest("User doesn't exist");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to add to roles");
            
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}
