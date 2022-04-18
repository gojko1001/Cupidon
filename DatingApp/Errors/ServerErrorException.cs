namespace DatingApp.Errors
{
    public class ServerErrorException : Exception
    {
        public ServerErrorException()
        {
        }

        public ServerErrorException(string message) : base(message)
        {
        }
    }
}
