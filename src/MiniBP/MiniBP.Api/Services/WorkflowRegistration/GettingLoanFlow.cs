using System;
using System.Collections.Generic;
using System.Linq;
using MiniBP.BPMS.Domain.Model.Workflow;
using MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;
using Stateless;

namespace Api.Services.WorkflowRegistration;

public class GettingLoanFlow : WorkFlow<GettingLoanSteps>
{
    public GettingLoanFlow(List<IFlowParameter> flowParameters)
        : base(flowParameters) { }

    public override string Name => "GettingLoanFlow";

    public override WorkflowStep<GettingLoanSteps> StartStep => Step_Apply;

    protected WorkflowStep<GettingLoanSteps> Step_Apply { get; private set; }
    protected WorkflowStep<GettingLoanSteps> Step_PrimitiveCheck { get; private set; }
    protected WorkflowStep<GettingLoanSteps> Step_PreparingDocuments { get; private set; }
    protected WorkflowStep<GettingLoanSteps> Step_Payment { get; private set; }

    protected override void RegistrationWorkflowSteps()
    {
        Step_Apply = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.Apply,
                                                        new CyclicAssignmentMethod(),
                                                        new List<Guid>() { Guid.NewGuid() },
                                                        string.Empty);

        Step_PrimitiveCheck = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.PrimitiveCheck,
                                                                 new CyclicAssignmentMethod(),
                                                                 new List<Guid>() { Guid.NewGuid() },
                                                                 string.Empty);

        Step_PreparingDocuments = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.PreparingDocuments,
                                                                     new CyclicAssignmentMethod(),
                                                                     new List<Guid>() { Guid.NewGuid() },
                                                                     string.Empty);

        Step_Payment = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.Payment,
                                                          new CyclicAssignmentMethod(),
                                                          new List<Guid>() { Guid.NewGuid() },
                                                          string.Empty, true);

        FlowHandler = new StateMachine<WorkflowStep<GettingLoanSteps>, WorkFlowActions>(Step_Apply);
        FlowHandler.Configure(StartStep)
           .OnEntry(() => { })
           .Permit(WorkFlowActions.Next, Step_PrimitiveCheck);

        FlowHandler.Configure(Step_PrimitiveCheck)
           .OnEntry(() => { })
           .Permit(WorkFlowActions.Next, Step_PreparingDocuments);

        FlowHandler.Configure(Step_PreparingDocuments)
           .OnEntry(() => { })
           .Permit(WorkFlowActions.Next, Step_Payment);
    }
}
