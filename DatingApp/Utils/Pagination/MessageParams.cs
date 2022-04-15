namespace DatingApp.Utils.Pagination
{
    public class MessageParams : PaginationParams
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Container { get; set; } = "Unread";
    }
}
