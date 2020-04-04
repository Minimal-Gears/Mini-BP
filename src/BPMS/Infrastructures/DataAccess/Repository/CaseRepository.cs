using BPMS.Domain.Model.Cartable;
using BPMS.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess.Repository
{
    public class CaseRepository : BaseRepository<Case>, ICaseRepository
    {
        public CaseRepository(BpmsDbContext context) : base(context)
        {
        }
    }
}