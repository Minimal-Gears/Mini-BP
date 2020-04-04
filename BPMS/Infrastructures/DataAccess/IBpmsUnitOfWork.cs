using System.Threading.Tasks;

namespace BPMS.Infrastructures.DataAccess
{
    public interface IBpmsUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
}