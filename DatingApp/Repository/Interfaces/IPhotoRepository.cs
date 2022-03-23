using DatingApp.DTOs;
using DatingApp.Entities;

namespace DatingApp.Repository.Interfaces
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhotoByIdAsync(int id);
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        void RemovePhoto(Photo photo);
    }
}
