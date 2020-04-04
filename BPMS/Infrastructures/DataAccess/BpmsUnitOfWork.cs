using System.Threading.Tasks;
using BPMS.Infrastructures.Helper;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess
{
    public sealed class BpmsUnitOfWork : IBpmsUnitOfWork
    {
        private readonly BpmsDbContext context;
        private readonly IDbExceptionHelper exceptionHelper;

        public BpmsUnitOfWork(BpmsDbContext context, IDbExceptionHelper exceptionHelper)
        {
            this.context = context;
            this.exceptionHelper = exceptionHelper;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw exceptionHelper.TranslateToException(ex);
            }
        }
    }
}