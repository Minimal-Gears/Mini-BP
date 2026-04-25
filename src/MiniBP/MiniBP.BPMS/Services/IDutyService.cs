using MiniBP.BPMS.Domain.Model.Cartable;

namespace MiniBP.BPMS.Domain.Services;

public interface IDutyService
{
    Duty GetDuty(Guid taskId);
}
