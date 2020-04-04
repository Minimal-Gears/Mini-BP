
using Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.Helper
{
    public interface IDbExceptionHelper
    {
        ExceptionResult Translate(DbUpdateException ex);
        BusinessException TranslateToException(DbUpdateException ex);
    }
}