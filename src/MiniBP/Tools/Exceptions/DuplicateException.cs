namespace Common.Exceptions
{
    public class DuplicateException : BusinessException
    {
        public DuplicateException(string message) : base(message)
        {
        }

        public DuplicateException(string message, string field) : base(message, field)
        {
        }
    }
}