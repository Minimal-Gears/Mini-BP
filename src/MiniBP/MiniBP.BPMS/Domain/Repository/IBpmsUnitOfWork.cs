namespace MiniBP.BPMS.Domain.Repository;

public interface IBpmsUnitOfWork
{
    void Commit();
    Task CommitAsync();
}