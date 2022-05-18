﻿using Microsoft.AspNetCore.Identity;

namespace DatingApp.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string KnownAs { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool PublicActivity { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<UserRelation> RelationByUsers { get; set; }
        public ICollection<UserRelation> RelationToUsers { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesRecieved { get; set; }
        public ICollection<AppUserRole> Roles { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
