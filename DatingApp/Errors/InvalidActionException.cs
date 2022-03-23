namespace DatingApp.Errors
{
    public class InvalidActionException : Exception
    {
        public InvalidActionException()
        {
        }

        public InvalidActionException(string message) : base(message)
        {
        }
    }
}
