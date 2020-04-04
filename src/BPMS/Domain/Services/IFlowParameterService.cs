using System.Collections.Generic;

namespace BPMS.Domain.Services
{
    public interface IFlowParameterService
    {
        string GetParameterValue(string parameterName);

        Dictionary<string, string> GetAllParameters();
    }
}