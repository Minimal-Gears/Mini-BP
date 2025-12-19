using System;

namespace Common.Exceptions
{
    public class BadDataException : BusinessException
    {
        public BadDataException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        public BadDataException(string message, string field, Exception innerException = null) : base(message, field, innerException)
        {
        }
    }
}