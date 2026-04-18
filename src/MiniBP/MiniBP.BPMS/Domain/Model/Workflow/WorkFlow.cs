using Stateless;

namespace MiniBP.BPMS.Domain.Model.Workflow;

public abstract class WorkFlow<TStep> where TStep : Enum
{
    protected WorkFlow(List<IFlowParameter> flowParameters)
    {
        //FlowHandler = new StateMachine<WorkflowStep<TStep>, WorkFlowActions>(initialState);
        FlowParameters = flowParameters;
        RegistrationWorkflowSteps();
    }

    protected StateMachine<WorkflowStep<TStep>, WorkFlowActions> FlowHandler { get; set; }

    public abstract string Name { get; }

    public abstract WorkflowStep<TStep> StartStep { get; }

    public List<IFlowParameter> FlowParameters { get; set; }

    public WorkflowStep<TStep> Next()
    {
        FlowHandler.Fire(WorkFlowActions.Next);
        return FlowHandler.State;
        //return WorkflowSteps.FirstOrDefault(a => Equals(a.Step, FlowHandler.State.Step) && Equals(a.IsFinal, FlowHandler.State.IsFinal));
    }

    //public List<WorkflowStep<TStep>> WorkflowSteps { get; set; }

    protected abstract void RegistrationWorkflowSteps();
}
