using MiniBP.BPMS.Domain.Model.Cartable;
using MiniBP.BPMS.Domain.Repository;

namespace MiniBP.Infrastructure.DataAccess.Repository;

public class CaseRepository : BaseRepository<Case>, ICaseRepository
{
    public CaseRepository(BpmsDbContext context) : base(context) { }
}
