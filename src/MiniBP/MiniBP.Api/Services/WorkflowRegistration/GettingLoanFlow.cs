using System;
using System.Collections.Generic;
using System.Linq;
using MiniBP.BPMS.Domain.Model.Workflow;
using MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;
using Stateless;

namespace Api.Services.WorkflowRegistration;

public class GettingLoanFlow : WorkFlow<GettingLoanSteps>
{
    public GettingLoanFlow(List<IFlowParameter> flowParameters, WorkflowStep<GettingLoanSteps>? initialState)
        : base(flowParameters, initialState) { }

    public override string Name => "GettingLoanFlow";

    private static Guid UserId => Guid.Parse("8c95a960-33d8-49d6-945d-693ce9db1419");

    public override WorkflowStep<GettingLoanSteps> StartStep => Step_Apply;

    protected WorkflowStep<GettingLoanSteps> Step_Apply { get; private set; }
    protected WorkflowStep<GettingLoanSteps> Step_PrimitiveCheck { get; private set; }
    protected WorkflowStep<GettingLoanSteps> Step_PreparingDocuments { get; private set; }
    protected WorkflowStep<GettingLoanSteps> Step_Payment { get; private set; }

    protected override void RegistrationWorkflowSteps(WorkflowStep<GettingLoanSteps>? initialState)
    {
        Step_Apply = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.Apply,
                                                        new FireBaseAssignmentMethod(),
                                                        new List<Guid>() { UserId },
                                                        string.Empty);

        Step_PrimitiveCheck = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.PrimitiveCheck,
                                                                 new CyclicAssignmentMethod(),
                                                                 new List<Guid>() { UserId },
                                                                 string.Empty);

        Step_PreparingDocuments = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.PreparingDocuments,
                                                                     new CyclicAssignmentMethod(),
                                                                     new List<Guid>() { UserId },
                                                                     string.Empty);

        Step_Payment = new WorkflowStep<GettingLoanSteps>(GettingLoanSteps.Payment,
                                                          new CyclicAssignmentMethod(),
                                                          new List<Guid>() { UserId },
                                                          string.Empty, true);

        initialState ??= Step_Apply;

        FlowHandler = new StateMachine<WorkflowStep<GettingLoanSteps>, WorkFlowActions>(initialState);
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
