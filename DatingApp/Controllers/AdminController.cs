using DatingApp.Entities;
using DatingApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.DTOs;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;

namespace DatingApp.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
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
            return Ok(await _unitOfWork.PhotoRepository.GetUnapprovedPhotos());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
            if (photo == null)
                return BadRequest("Photo not found");

            photo.IsApproved = true;
            
            var user = await _unitOfWork.UserRepository.GetUserByPhotoIdAsync(photoId);
            if(user != null && !user.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            if(await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to approve photo");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
            if (photo == null)
                return BadRequest("Photo not found");
            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error == null)
                    _unitOfWork.PhotoRepository.DeletePhoto(photo);
            }
            else
            {
                _unitOfWork.PhotoRepository.DeletePhoto(photo);
            }

            if(await _unitOfWork.Complete())
                return Ok();
            return BadRequest("Failed to delete photo");
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
