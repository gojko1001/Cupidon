using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Entities
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
    }
}
