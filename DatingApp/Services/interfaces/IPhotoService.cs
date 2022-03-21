using DatingApp.DTOs;
using DatingApp.Entities;

namespace DatingApp.Services.interfaces
{
    public interface IPhotoService
    {
        Task<Photo> AddPhoto(IFormFile file, int userId);
        Task SetMainPhoto(int photoId, int userId);
        Task RemovePhoto(int photoId, int userId);
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task ApprovePhoto(int photoId);
        Task RejectPhoto(int photoId);
    }
}
