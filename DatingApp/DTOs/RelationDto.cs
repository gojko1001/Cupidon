using DatingApp.Entities;
using DatingApp.Extensions;
using System.Linq.Expressions;

namespace DatingApp.DTOs
{
    public class RelationDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }

        public static Expression<Func<AppUser, RelationDto>> UserToRelationSelector
        {
            get
            {
                return user => new RelationDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    KnownAs = user.KnownAs,
                    Age = user.DateOfBirth.CalculateAge(),
                    PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                    City = user.City
                };
            }
        }
    }
}
