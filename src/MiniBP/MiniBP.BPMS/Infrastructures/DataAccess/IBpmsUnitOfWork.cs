namespace MiniBP.BPMS.Infrastructures.DataAccess;

public interface IBpmsUnitOfWork
{
    void Commit();
    Task CommitAsync();
}