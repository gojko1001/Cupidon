namespace DatingApp.Utils.Pagination
{
    public class RelationParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
