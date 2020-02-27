namespace BPMS.Infrastructures.Helper
{
    public struct ExceptionResult
    {
        public ExceptionResult(DbExceptionCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public DbExceptionCode Code { get; }
        public string Message { get; }
    }
}