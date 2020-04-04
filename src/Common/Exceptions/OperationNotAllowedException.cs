namespace Common.Exceptions
{
    public class OperationNotAllowedException : BusinessException
    {
        public OperationNotAllowedException(string message) : base(message)
        {
        }

        public OperationNotAllowedException(string message, string field) : base(message, field)
        {
        }
    }
}