using BPMS.Domain.Model.Workflow;

namespace BPMS.Domain.Model.Cartable
{
    public class FlowParameter : IEntity, IFlowParameter
    {
        public FlowParameter(int caseId, string key, string value)
        {
            CaseId = caseId;
            Key = key;
            Value = value;
        }

        private FlowParameter()
        {
        }

        public int Id { get; private set; }

        public int CaseId { get; private set; }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public Case Case { get; private set; }
    }
}