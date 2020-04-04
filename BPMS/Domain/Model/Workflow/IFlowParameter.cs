namespace BPMS.Domain.Model.Workflow
{
    public interface IFlowParameter
    {
        string Key { get; }

        string Value { get; }
    }
}