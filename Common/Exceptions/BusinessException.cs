using System;

namespace Common.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        public BusinessException(string message, string field, Exception innerException = null) : this(message, innerException)
        {
            Field = field;
        }

        public BusinessException(string message, int errorCode, Exception innerException = null) : this(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public string Field { get; }
        public int ErrorCode { get; }
    }
}