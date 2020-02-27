using System;
using System.Collections.Generic;
using System.Linq;
using Stateless;

namespace BPMS.Services.WorkFlow
{
    public class WorkflowStep<TStep> where TStep : Enum
    {
        public WorkflowStep(TStep step,bool isFinal = false)
        {
            Step = step;
            IsFinal = isFinal;
        }
        public TStep Step { get; }

        public bool IsFinal { get; }
        
    }
    public abstract class WorkFlow<TStep> : StateMachine<WorkflowStep<TStep>, WorkFlowActions> where TStep : Enum
    {
        protected WorkFlow(WorkflowStep<TStep> initialState) : base(initialState)
        {
            AssignmentRules = RegistrationAssignmentRules();
        }

        public abstract string Name { get; }

        public abstract WorkflowStep<TStep> StartStep { get; }

        public AssignmentRule<TStep> Next()
        {
            Fire(WorkFlowActions.Next);
            return  AssignmentRules.FirstOrDefault(a => Equals(a.Step, State));
        }

        public IList<AssignmentRule<TStep>> AssignmentRules { get; }

        protected abstract List<AssignmentRule<TStep>> RegistrationAssignmentRules();
    }
}