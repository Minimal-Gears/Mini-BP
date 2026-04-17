using Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MiniBP.Infrastructure.Helper;

public interface IDbExceptionHelper
{
    ExceptionResult Translate(DbUpdateException ex);

    BusinessException TranslateToException(DbUpdateException ex);
}