using System.Collections.Generic;

namespace BPMS.Services.CartableService
{
    public class RouteVariable
    {
        public int CaseId { get; set; }

        public Dictionary<string, string> WorkflowParameters { get; set; }

        //  public List<Input> WorkflowParameters { get; set; }
    }

    public class Input
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}