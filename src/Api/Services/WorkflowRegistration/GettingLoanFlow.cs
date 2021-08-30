using System;
using System.Collections.Generic;
using System.Linq;
using BPMS.Domain.Model.Workflow;
using BPMS.Domain.Model.Workflow.AssignmentMethod;

namespace Api.Services.WorkflowRegistration
{
    public class GettingLoanFlow : WorkFlow<GettingLoanSteps>
    {
        public GettingLoanFlow(WorkflowStep<GettingLoanSteps> initialState, List<IFlowParameter> flowParameters) : base(
            initialState, flowParameters)
        {
            this.Configure(StartStep)
                .OnEntry(() => { })
                .Permit(WorkFlowActions.Next, WorkflowSteps.Single(a => a.Step == GettingLoanSteps.PrimitiveCheck));

            this.Configure(WorkflowSteps.Single(a => a.Step == GettingLoanSteps.PrimitiveCheck))
                .OnEntry(() => { })
                .Permit(WorkFlowActions.Next, WorkflowSteps.Single(a => a.Step == GettingLoanSteps.PreparingDocuments));

            this.Configure(WorkflowSteps.Single(a => a.Step == GettingLoanSteps.PreparingDocuments))
                .OnEntry(() => { })
                .Permit(WorkFlowActions.Next, WorkflowSteps.Single(a => a.Step == GettingLoanSteps.Payment));
        }

        public override string Name => "GettingLoanFlow";

        public override WorkflowStep<GettingLoanSteps> StartStep =>
            WorkflowSteps.Single(a => a.Step == GettingLoanSteps.Apply);

        protected override List<WorkflowStep<GettingLoanSteps>> RegistrationWorkflowSteps()
        {
            var workflowSteps = new List<WorkflowStep<GettingLoanSteps>>();

            workflowSteps.Add(new WorkflowStep<GettingLoanSteps>(
                GettingLoanSteps.Apply,
                new CyclicAssignmentMethod(),
                new List<Guid>() {Guid.NewGuid()},
                string.Empty));

            workflowSteps.Add(new WorkflowStep<GettingLoanSteps>(
                GettingLoanSteps.PrimitiveCheck,
                new CyclicAssignmentMethod(),
                new List<Guid>() {Guid.NewGuid()},
                string.Empty));

            workflowSteps.Add(new WorkflowStep<GettingLoanSteps>(
                GettingLoanSteps.PreparingDocuments,
                new CyclicAssignmentMethod(),
                new List<Guid>() {Guid.NewGuid()},
                string.Empty));

            workflowSteps.Add(new WorkflowStep<GettingLoanSteps>(
                GettingLoanSteps.Payment,
                new CyclicAssignmentMethod(),
                new List<Guid>() {Guid.NewGuid()},
                string.Empty));

            return workflowSteps;
        }
    }
}