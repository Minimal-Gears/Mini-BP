using System.Collections.Generic;

namespace BPMS.Services.CartableService
{
    public class RouteVariable
    {
        public int CaseId { get; set; }

        public Dictionary<string, string> WorkflowParameter { get; set; }
    }
}