using MiniBP.BPMS.Domain.Model.Cartable;

namespace MiniBP.BPMS.Domain.Services;

public interface ICaseService
{
    Case StartCase(string workflowName, IDictionary<string, object> flowParameters);
}
