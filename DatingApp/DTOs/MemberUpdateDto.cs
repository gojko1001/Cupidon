using System.ComponentModel.DataAnnotations;

namespace DatingApp.DTOs
{
    public class MemberUpdateDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool PublicActivity { get; set; }
    }
}
