using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BPMS.Infrastructures.Helper
{
    public class PostgresExceptionHelper : IDbExceptionHelper
    {
        public ExceptionResult Translate(DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException postgresException)
            {
                if (postgresException.SqlState == "23050")
                {
                    return new ExceptionResult(DbExceptionCode.Duplicate, postgresException.Detail);
                }
            }

            throw ex;
        }

        private const string DuplicateKey = "23505";
        private const string NullValue = "23502";

        public BusinessException TranslateToException(DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException postgresException)
            {
                switch (postgresException.SqlState)
                {
                    case DuplicateKey:
                        return new DuplicateException(postgresException.MessageText, postgresException.Detail);
                    case NullValue:
                        return new BadDataException(postgresException.MessageText, postgresException.ColumnName);
                }
            }

            throw ex;
        }
    }
}