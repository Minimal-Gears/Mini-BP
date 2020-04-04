using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;

namespace BPMS.Domain.Model.Workflow
{
    public abstract class WorkFlow<TStep> : StateMachine<WorkflowStep<TStep>, WorkFlowActions> where TStep : Enum
    {
        protected WorkFlow(WorkflowStep<TStep> initialState, List<IFlowParameter> flowParameters) : base(initialState)
        {
            FlowParameters = flowParameters;
            WorkflowSteps = RegistrationWorkflowSteps();
        }

        public abstract string Name { get; }

        public abstract WorkflowStep<TStep> StartStep { get; }

        public List<IFlowParameter> FlowParameters { get; set; }

        public WorkflowStep<TStep> Next()
        {
            Fire(WorkFlowActions.Next);
            return WorkflowSteps.FirstOrDefault(a => Equals(a.Step, State.Step) && Equals(a.IsFinal, State.IsFinal));
        }

        public List<WorkflowStep<TStep>> WorkflowSteps { get; set; }

        protected abstract List<WorkflowStep<TStep>> RegistrationWorkflowSteps();
    }
}