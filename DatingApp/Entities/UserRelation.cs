namespace DatingApp.Entities
{
    public class UserRelation
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser RelatedUser { get; set; }
        public int RelatedUserId { get; set; }
        public RelationStatus Relation { get; set; }
    }
}
