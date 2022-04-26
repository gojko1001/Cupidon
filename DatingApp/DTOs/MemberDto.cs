using System.Linq.Expressions;

namespace DatingApp.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhotoUrl { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string RelationTo { get; set; }
        public bool? PublicActivity { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }

        public static Expression<Func<MemberDto, MemberDto>> BlockedUserSelector
        {
            get
            {
                return u => new MemberDto()
                {
                    Id = u.Id,
                    Age = u.Age,
                    Username = u.Username,
                    KnownAs = u.KnownAs,
                    PhotoUrl = u.PhotoUrl
                };
            }
        }

        public static Expression<Func<MemberDto, MemberDto>> NonLogedUserSelector
        {
            get
            {
                return u => new MemberDto()
                {
                    Id = u.Id,
                    Username = u.Username,
                    PhotoUrl = u.PhotoUrl,
                    Age = u.Age,
                    KnownAs = u.KnownAs,
                    Created = u.Created,
                    LastActive = u.LastActive,
                    City = u.City,
                    Country = u.Country,
                    Gender = u.Gender,
                    Interests = u.Interests,
                    Introduction = u.Introduction,
                    LookingFor = u.LookingFor,
                    Photos = u.Photos,
                    RelationTo = u.RelationTo
                };
            }
        }
    }
}
