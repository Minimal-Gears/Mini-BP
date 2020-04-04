using System.Threading.Tasks;

namespace BPMS.Infrastructures.DataAccess
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();
    }
}