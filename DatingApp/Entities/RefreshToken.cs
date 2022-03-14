using System.ComponentModel.DataAnnotations;

namespace DatingApp.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
    }
}
